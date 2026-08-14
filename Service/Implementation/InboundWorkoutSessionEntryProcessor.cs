using System.Text.Json;
using Domain.Dto;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InboundWorkoutSessionEntryProcessor
{
    private readonly IRepository<InboundWorkoutSessionEntry> _entryRepository;
    private readonly IRepository<WorkoutSession> _sessionRepository;
    private readonly IMemberService _memberService;
    private readonly ITrainerService _trainerService;
    private readonly IExerciseService _exerciseService;
    private readonly IMembershipService _membershipService;
    private readonly ILogger<InboundWorkoutSessionEntryProcessor> _logger;
    
    public InboundWorkoutSessionEntryProcessor(
        IRepository<InboundWorkoutSessionEntry> entryRepository,
        IRepository<WorkoutSession> sessionRepository,
        IMemberService memberService,
        ITrainerService trainerService,
        IExerciseService exerciseService,
        ILogger<InboundWorkoutSessionEntryProcessor> logger, IMembershipService membershipService)
    {
        _entryRepository = entryRepository;
        _sessionRepository = sessionRepository;
        _memberService = memberService;
        _trainerService = trainerService;
        _exerciseService = exerciseService;
        _logger = logger;
        _membershipService = membershipService;
    }

    public async Task ProcessPendingEntriesAsync()
    {
        var pending = await _entryRepository.GetAllAsync(
            selector: x => x,
            predicate: x => x.Status == InboundWorkoutSessionStatus.Pending,
            orderBy: x => x.OrderBy(e => e.ReceivedAt),
            take: 10);

        foreach (var entry in pending)
        {
            try
            {
                entry.Status = InboundWorkoutSessionStatus.Processing;
                await _entryRepository.UpdateAsync(entry);
                
                await ProcessEntryAsync(entry);
                
                entry.Status = InboundWorkoutSessionStatus.Completed;
                entry.ProcessedAt = DateTime.UtcNow;
                
                _logger.LogInformation("Processed inbound entry {Id} -> WorkoutSession {SessionId}", entry.Id, entry.CreatedWorkoutSessionId);
            }
            catch (Exception ex)
            {
                entry.Status = InboundWorkoutSessionStatus.Failed;
                entry.ErrorMessage = ex.Message;
                entry.ProcessedAt = DateTime.UtcNow;

                _logger.LogError(ex, "Failed to process inbound entry {Id}", entry.Id);
            }
            
            await _entryRepository.UpdateAsync(entry);
        }
    }

    private async Task ProcessEntryAsync(InboundWorkoutSessionEntry entry)
    {
        var request = JsonSerializer.Deserialize<InboundWorkoutSessionRequest>(entry.RawPayload);
        if (request == null)
            throw new InvalidOperationException("Failed to deserialize payload");
        
        var member = await _memberService.GetByEmailAsync(request.MemberEmail);
        if (member == null)
            throw new InvalidOperationException($"Member with email {request.MemberEmail} not found");
        
        var activeMembership = await _membershipService.GetActiveByMemberIdAsync(member.Id);
        if (activeMembership == null)
            throw new InvalidOperationException($"Member {member.Id} does not have an active membership");
        
        var trainer = await _trainerService.GetByNameAsync(request.TrainerName);
        if (trainer == null)
            throw new InvalidOperationException($"Trainer {request.TrainerName} not found");
        
        var exercise = await _exerciseService.GetByNameAsync(request.ExerciseName);
        if (exercise == null)
            throw new InvalidOperationException($"Exercise {request.ExerciseName} not found");
        
        var duplicates = await _sessionRepository.Get(
            selector:x=>x,
            predicate:x=>x.MemberId==member.Id && x.TrainerId==trainer.Id
            && x.ExerciseId==exercise.Id && x.Date.Date == request.SessionDate.Date);
        
        if (duplicates != null)
            throw new InvalidOperationException("Duplicate workout session");
        
        var session = new WorkoutSession
        {
            MemberId = member.Id,
            TrainerId = trainer.Id,
            ExerciseId = exercise.Id,
            Date = request.SessionDate,
            SetsCompleted = request.SetsCompleted,
            RepsCompleted = request.RepsCompleted,
            WeightUsedKg =  request.WeightUsedKg,
            DurationMinutes = request.DurationMinutes,
            Notes = request.Notes
        };
        
        var created = await _sessionRepository.InsertAsync(session);

        entry.CreatedWorkoutSessionId = created.Id;
    }
}
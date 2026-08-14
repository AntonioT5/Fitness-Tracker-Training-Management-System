using System.Security.Authentication;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class WorkoutSessionService : IWorkoutSessionService
{
    private readonly IRepository<WorkoutSession> _repository;
    private readonly IMembershipService _membershipService;
    private readonly ITrainerService _trainerService;
    private readonly IMemberService _memberService;
    private readonly IExerciseService _exerciseService;
    
    public WorkoutSessionService(IRepository<WorkoutSession> repository, IMembershipService membershipService, ITrainerService trainerService, IMemberService memberService, IExerciseService exerciseService)
    {
        _repository = repository;
        _membershipService = membershipService;
        _trainerService = trainerService;
        _memberService = memberService;
        _exerciseService = exerciseService;
    }
    
    public async Task<List<WorkoutSession>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x,
            include: x=>x.Include(e=>e.Member).ThenInclude(e=>e.User)
                .Include(e=>e.Trainer).ThenInclude(e=>e.User)
                .Include(e=>e.Exercise));
        return result.ToList();
    }

    public async Task<WorkoutSession?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id,
            include: x=>x.Include(e=>e.Member).ThenInclude(e=>e.User)
                .Include(e=>e.Trainer).ThenInclude(e=>e.User)
                .Include(e=>e.Exercise));
    }

    public async Task<WorkoutSession> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"WorkoutSession with id {id} not found");
        }
        return result;
    }

    public async Task<WorkoutSession> InsertAsync(WorkoutSessionDto workoutSessionDto)
    {
        
        var memebr = await _memberService.GetByIdAsync(workoutSessionDto.MemberId);
        if (memebr == null)
            throw new InvalidOperationException($"Member with id {workoutSessionDto.MemberId} not found");
        
        var trainer = await _trainerService.GetByIdAsync(workoutSessionDto.TrainerId);
        if (trainer == null)
            throw new InvalidOperationException($"Trainer with id {workoutSessionDto.TrainerId} not found");
        
        var exercise = await _exerciseService.GetByIdAsync(workoutSessionDto.ExerciseId);
        if (exercise == null)
            throw new InvalidOperationException($"Exercise with id {workoutSessionDto.TrainerId} not found");
        
        var activeMembership = await _membershipService.GetActiveByMemberIdAsync(workoutSessionDto.MemberId);
        if (activeMembership == null)
            throw new InvalidOperationException($"Member {workoutSessionDto.MemberId} does not have an active membership");
        
        var result = new WorkoutSession()
        {
            Date = workoutSessionDto.Date,
            SetsCompleted = workoutSessionDto.SetsCompleted,
            RepsCompleted = workoutSessionDto.RepsCompleted,
            WeightUsedKg = workoutSessionDto.WeightUsedKg,
            DurationMinutes = workoutSessionDto.DurationMinutes,
            Notes = workoutSessionDto.Notes,
            MemberId = workoutSessionDto.MemberId,
            TrainerId = workoutSessionDto.TrainerId,
            ExerciseId = workoutSessionDto.ExerciseId,
        };
        await _repository.InsertAsync(result);
        return await GetByIdNotNullAsync(result.Id);
    }

    public async Task<WorkoutSession> UpdateAsync(Guid id, WorkoutSessionDto workoutSessionDto)
    {
        var result = await GetByIdNotNullAsync(id);
        
        var memebr = await _memberService.GetByIdAsync(workoutSessionDto.MemberId);
        if (memebr == null)
            throw new InvalidOperationException($"Member with id {workoutSessionDto.MemberId} not found");
        
        var trainer = await _trainerService.GetByIdAsync(workoutSessionDto.TrainerId);
        if (trainer == null)
            throw new InvalidOperationException($"Trainer with id {workoutSessionDto.TrainerId} not found");
        
        var exercise = await _exerciseService.GetByIdAsync(workoutSessionDto.ExerciseId);
        if (exercise == null)
            throw new InvalidOperationException($"Exercise with id {workoutSessionDto.TrainerId} not found");
        
        var activeMembership = await _membershipService.GetActiveByMemberIdAsync(workoutSessionDto.MemberId);
        if (activeMembership == null)
            throw new InvalidOperationException($"Member {workoutSessionDto.MemberId} does not have an active membership");
        
        result.Date = workoutSessionDto.Date;
        result.SetsCompleted = workoutSessionDto.SetsCompleted;
        result.RepsCompleted = workoutSessionDto.RepsCompleted;
        result.WeightUsedKg = workoutSessionDto.WeightUsedKg;
        result.DurationMinutes = workoutSessionDto.DurationMinutes;
        result.Notes = workoutSessionDto.Notes;
        result.MemberId = workoutSessionDto.MemberId;
        result.TrainerId = workoutSessionDto.TrainerId;
        result.ExerciseId = workoutSessionDto.ExerciseId;
        return await _repository.UpdateAsync(result);
    }

    public async Task<WorkoutSession> DeleteAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(result);
    }
    
    public async Task<PaginatedResult<WorkoutSession>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x=>x.Include(e=>e.Member).ThenInclude(e=>e.User)
                .Include(e=>e.Trainer).ThenInclude(e=>e.User)
                .Include(e=>e.Exercise),
            orderBy: x=>x.OrderBy(e=>e.Date),
            asNoTracking: true);
    }

    public async Task<List<WorkoutSession>> GetAllByMemberNameAsync(Guid memberId)
    {
        var result = await _repository.GetAllAsync(x => x,
            predicate:x=>x.MemberId == memberId,
            include: x=>x.Include(e=>e.Member).ThenInclude(e=>e.User)
                .Include(e=>e.Trainer).ThenInclude(e=>e.User)
                .Include(e=>e.Exercise));
        return result.ToList();
    }

    public async Task<ICollection<WorkoutSession>> AddRangeAsync(List<WorkoutSession> ws)
    {
        return await _repository.InsertManyAsync(ws);
    }
}
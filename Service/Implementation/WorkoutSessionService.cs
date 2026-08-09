using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class WorkoutSessionService : IWorkoutSessionService
{
    private readonly IRepository<WorkoutSession> _repository;
    
    public WorkoutSessionService(IRepository<WorkoutSession> repository)
    {
        _repository = repository;
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
        return await _repository.InsertAsync(result);
    }

    public async Task<WorkoutSession> UpdateAsync(Guid id, WorkoutSessionDto workoutSessionDto)
    {
        var result = await GetByIdNotNullAsync(id);
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
}
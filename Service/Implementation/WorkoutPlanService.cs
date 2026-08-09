using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class WorkoutPlanService : IWorkoutPlanService
{
    private readonly IRepository<WorkoutPlan> _repository;
    
    public WorkoutPlanService(IRepository<WorkoutPlan> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<WorkoutPlan>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x,
            include:x=>x.Include(e=>e.Trainer).ThenInclude(e=>e.User));
        return result.ToList();
    }

    public async Task<WorkoutPlan?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id,
            include:x=>x.Include(e=>e.Trainer).ThenInclude(e=>e.User));
    }

    public async Task<WorkoutPlan> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"WorkoutPlan with id {id} not found");
        }
        return result;
    }

    public async Task<WorkoutPlan> InsertAsync(WorkoutPlanDto workoutPlanDto)
    {
        var result = new WorkoutPlan()
        {
            Name = workoutPlanDto.Name,
            Description = workoutPlanDto.Description,
            DurationWeeks = workoutPlanDto.DurationWeeks,
            TrainerId = workoutPlanDto.TrainerId
        };
        return await _repository.InsertAsync(result);
    }

    public async Task<WorkoutPlan> UpdateAsync(Guid id, WorkoutPlanDto workoutPlanDto)
    {
        var result = await GetByIdNotNullAsync(id);
        result.Name = workoutPlanDto.Name;
        result.Description = workoutPlanDto.Description;
        result.DurationWeeks = workoutPlanDto.DurationWeeks;
        result.TrainerId = workoutPlanDto.TrainerId;
        return await _repository.UpdateAsync(result);
    }

    public async Task<WorkoutPlan> DeleteAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(result);
    }
    
    public async Task<PaginatedResult<WorkoutPlan>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include:x=>x.Include(e=>e.Trainer).ThenInclude(e=>e.User),
            orderBy: x=>x.OrderBy(e=>e.Name),
            asNoTracking: true);
    }
}
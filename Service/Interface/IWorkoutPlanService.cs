using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IWorkoutPlanService
{
    Task<List<WorkoutPlan>> GetAllAsync();
    Task<WorkoutPlan?> GetByIdAsync(Guid id);
    Task<WorkoutPlan> GetByIdNotNullAsync(Guid id);
    Task<WorkoutPlan> InsertAsync(WorkoutPlanDto workoutPlanDto);
    Task<WorkoutPlan> UpdateAsync(Guid id, WorkoutPlanDto workoutPlanDto);
    Task<WorkoutPlan> DeleteAsync(Guid id);
    
    public Task<PaginatedResult<WorkoutPlan>> GetAllPagedAsync(int pageNumber, int pageSize);
    
}
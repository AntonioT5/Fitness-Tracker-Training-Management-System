using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IExerciseWorkoutPlanService
{
    Task<List<ExerciseWorkoutPlan>> GetAllAsync();
    Task<ExerciseWorkoutPlan?> GetByIdAsync(Guid id);
    Task<ExerciseWorkoutPlan> GetByIdNotNullAsync(Guid id);
    Task<ExerciseWorkoutPlan> InsertAsync(ExerciseWorkoutPlanDto exerciseWorkoutPlanDto);
    Task<ExerciseWorkoutPlan> UpdateAsync(Guid id, ExerciseWorkoutPlanDto exerciseWorkoutPlanDto);
    Task<ExerciseWorkoutPlan> DeleteAsync(Guid id);
}
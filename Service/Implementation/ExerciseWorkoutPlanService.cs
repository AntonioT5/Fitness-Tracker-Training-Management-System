using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ExerciseWorkoutPlanService : IExerciseWorkoutPlanService
{
    private readonly IRepository<ExerciseWorkoutPlan> _repository;

    public ExerciseWorkoutPlanService(IRepository<ExerciseWorkoutPlan> repository)
    {
        _repository = repository;
    }

    public async Task<List<ExerciseWorkoutPlan>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x,
            include:x=>x.Include(e => e.Exercise)
                .Include(e=>e.WorkoutPlan));
        return result.ToList();
    }

    public async Task<ExerciseWorkoutPlan?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id,
            include:x=>x.Include(e => e.Exercise)
                .Include(e=>e.WorkoutPlan));
    }

    public async Task<ExerciseWorkoutPlan> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"ExerciseWorkoutPlan with id {id} not found");
        }
        
        return result;
    }

    public async Task<ExerciseWorkoutPlan> InsertAsync(ExerciseWorkoutPlanDto exerciseWorkoutPlanDto)
    {
        var exerciseWorkoutPlan = new ExerciseWorkoutPlan()
        {
            Sets = exerciseWorkoutPlanDto.Sets,
            Reps = exerciseWorkoutPlanDto.Reps,
            ExerciseId = exerciseWorkoutPlanDto.ExerciseId,
            WorkoutPlanId = exerciseWorkoutPlanDto.WorkoutPlanId
        };
        return await _repository.InsertAsync(exerciseWorkoutPlan);
    }

    public async Task<ExerciseWorkoutPlan> UpdateAsync(Guid id, ExerciseWorkoutPlanDto exerciseWorkoutPlanDto)
    {
        var exerciseWorkoutPlan = await GetByIdNotNullAsync(id);
        exerciseWorkoutPlan.Sets = exerciseWorkoutPlanDto.Sets;
        exerciseWorkoutPlan.Reps = exerciseWorkoutPlanDto.Reps;
        exerciseWorkoutPlan.ExerciseId = exerciseWorkoutPlanDto.ExerciseId;
        exerciseWorkoutPlan.WorkoutPlanId = exerciseWorkoutPlanDto.WorkoutPlanId;
        return await _repository.UpdateAsync(exerciseWorkoutPlan);
    }

    public async Task<ExerciseWorkoutPlan> DeleteAsync(Guid id)
    {
        var exerciseWorkoutPlan = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(exerciseWorkoutPlan);
    }
}
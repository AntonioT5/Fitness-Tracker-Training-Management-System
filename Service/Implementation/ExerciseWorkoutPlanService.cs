using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ExerciseWorkoutPlanService : IExerciseWorkoutPlanService
{
    private readonly IRepository<ExerciseWorkoutPlan> _repository;
    private readonly IExerciseService _exerciseService;
    private readonly IWorkoutPlanService _workoutPlanService;
    
    public ExerciseWorkoutPlanService(IRepository<ExerciseWorkoutPlan> repository, IExerciseService exerciseService, IWorkoutPlanService workoutPlanService)
    {
        _repository = repository;
        _exerciseService = exerciseService;
        _workoutPlanService = workoutPlanService;
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
        var exercise = await _exerciseService.GetByIdAsync(exerciseWorkoutPlanDto.ExerciseId);
        if (exercise == null)
            throw new InvalidOperationException($"Exercise with {exerciseWorkoutPlanDto.ExerciseId} not found");
        
        var workoutPlan = await _workoutPlanService.GetByIdAsync(exerciseWorkoutPlanDto.WorkoutPlanId);
        if (workoutPlan == null)
            throw new InvalidOperationException($"WorkoutPlan with id {exerciseWorkoutPlanDto.WorkoutPlanId} not found");
        
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
        var exercise = await _exerciseService.GetByIdAsync(exerciseWorkoutPlanDto.ExerciseId);
        if (exercise == null)
            throw new InvalidOperationException($"Exercise with {exerciseWorkoutPlanDto.ExerciseId} not found");
        
        var workoutPlan = await _workoutPlanService.GetByIdAsync(exerciseWorkoutPlanDto.WorkoutPlanId);
        if (workoutPlan == null)
            throw new InvalidOperationException($"WorkoutPlan with id {exerciseWorkoutPlanDto.WorkoutPlanId} not found");
        
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
    
    public async Task<PaginatedResult<ExerciseWorkoutPlan>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include:x=>x.Include(e => e.Exercise)
                .Include(e=>e.WorkoutPlan),
            orderBy: x=>x.OrderBy(e=>e.Exercise.Name), 
            asNoTracking: true);
    }
}
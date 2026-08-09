using Domain.Dto;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ExerciseService : IExerciseService
{
    private readonly IRepository<Exercise> _repository;
    
    public ExerciseService(IRepository<Exercise> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Exercise>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x=>x);
        return result.ToList();
    }

    public async Task<Exercise?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(
            selector:x=>x, 
            predicate:x=>x.Id == id);
    }

    public async Task<Exercise> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if(result == null)
        {
            throw new InvalidOperationException($"Exercise with id {id} not found");
        }
        
        return result;
    }

    public async Task<Exercise> InsertAsync(ExerciseDto exerciseDto)
    {
        
        var exercise = new Exercise()
        {
            Name = exerciseDto.Name,
            MuscleGroup = exerciseDto.MuscleGroup,
            Difficulty = exerciseDto.Difficulty,
            Equipment = exerciseDto.Equipment,
            Description = exerciseDto.Description
        };
        return await _repository.InsertAsync(exercise);
    }

    public async Task<Exercise> UpdateAsync(Guid id, ExerciseDto exerciseDto)
    {
        var exercise = await GetByIdNotNullAsync(id);
        exercise.Name = exerciseDto.Name;
        exercise.MuscleGroup = exerciseDto.MuscleGroup;
        exercise.Difficulty = exerciseDto.Difficulty;
        exercise.Equipment = exerciseDto.Equipment;
        exercise.Description = exerciseDto.Description;
        return await _repository.UpdateAsync(exercise);
    }

    public async Task<Exercise> DeleteAsync(Guid id)
    {
        var exercise = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(exercise);
    }

    public async Task<PaginatedResult<Exercise>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: x=>x.OrderBy(e=>e.Name),
            asNoTracking: true);
    }
}
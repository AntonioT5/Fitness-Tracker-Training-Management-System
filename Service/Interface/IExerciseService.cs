using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IExerciseService
{
    Task<List<Exercise>> GetAllAsync();
    Task<Exercise?> GetByIdAsync(Guid id);
    Task<Exercise> GetByIdNotNullAsync(Guid id);
    Task<Exercise> InsertAsync(ExerciseDto exerciseDto);
    Task<Exercise> UpdateAsync(Guid id, ExerciseDto exerciseDto);
    Task<Exercise> DeleteAsync(Guid id);
    
    public Task<PaginatedResult<Exercise>> GetAllPagedAsync(int pageNumber, int pageSize);
}
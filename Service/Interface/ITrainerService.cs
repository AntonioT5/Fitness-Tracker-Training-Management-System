using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface ITrainerService
{
    Task<List<Trainer>> GetAllAsync();
    Task<Trainer?> GetByIdAsync(Guid id);
    Task<Trainer> GetByIdNotNullAsync(Guid id);
    Task<Trainer> InsertAsync(TrainerDto trainerDto);
    Task<Trainer> UpdateAsync(Guid id, TrainerDto trainerDto);
    Task<Trainer> DeleteAsync(Guid id);
    
    public Task<PaginatedResult<Trainer>> GetAllPagedAsync(int pageNumber, int pageSize);
    
    Task<Trainer?> GetByNameAsync(string name);
    
}
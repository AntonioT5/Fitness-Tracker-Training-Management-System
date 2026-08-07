using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IGymService
{
    Task<List<Gym>> GetAllAsync();
    Task<Gym?> GetByIdAsync(Guid id);
    Task<Gym> GetByIdNotNullAsync(Guid id);
    Task<Gym> InsertAsync(GymDto gymDto);
    Task<Gym> UpdateAsync(Guid id, GymDto gymDto);
    Task<Gym> DeleteAsync(Guid id);
}
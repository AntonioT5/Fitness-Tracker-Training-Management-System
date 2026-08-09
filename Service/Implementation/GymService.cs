using Domain.Dto;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class GymService : IGymService
{
    private readonly IRepository<Gym> _repository;
    
    public GymService(IRepository<Gym> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Gym>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x);
        return result.ToList();
    }

    public async Task<Gym?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id);
    }

    public async Task<Gym> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"Gym with id {id} not found");
        }
        return result;
    }

    public async Task<Gym> InsertAsync(GymDto gymDto)
    {
        var gym = new Gym()
        {
            Name = gymDto.Name,
            Address = gymDto.Address,
            Phone = gymDto.Phone,
            Email = gymDto.Email
        };
        return await _repository.InsertAsync(gym);
    }

    public async Task<Gym> UpdateAsync(Guid id, GymDto gymDto)
    {
        var result = await GetByIdNotNullAsync(id);
        result.Name = gymDto.Name;
        result.Address = gymDto.Address;
        result.Phone = gymDto.Phone;
        result.Email = gymDto.Email;
        return await _repository.UpdateAsync(result);
    }

    public async Task<Gym> DeleteAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(result);
    }
    
    public async Task<PaginatedResult<Gym>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: x=>x.OrderBy(e=>e.Name),
            asNoTracking: true);
    }
}
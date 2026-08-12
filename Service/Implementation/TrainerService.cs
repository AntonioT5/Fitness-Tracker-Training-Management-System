using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TrainerService : ITrainerService
{
    private readonly IRepository<Trainer> _repository;
    
    public TrainerService(IRepository<Trainer> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Trainer>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x,
            include: x=>x.Include(e=>e.User));
        return result.ToList();
    }

    public async Task<Trainer?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id,
            include: x=>x.Include(e=>e.User));
    }

    public async Task<Trainer> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"Trainer with id {id} not found");
        }
        return result;
    }

    public async Task<Trainer> InsertAsync(TrainerDto trainerDto)
    {
        var result = new Trainer()
        {
            UserId = trainerDto.UserId,
            YearsExperience = trainerDto.YearsExperience
        };
        return await _repository.InsertAsync(result);
    }

    public async Task<Trainer> UpdateAsync(Guid id, TrainerDto trainerDto)
    {
        var result = await GetByIdNotNullAsync(id);
        result.UserId = trainerDto.UserId;
        result.YearsExperience = trainerDto.YearsExperience;
        return await _repository.UpdateAsync(result);
    }

    public async Task<Trainer> DeleteAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(result);
    }
    
    public async Task<PaginatedResult<Trainer>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x=>x.Include(e=>e.User),
            orderBy: x=>x.OrderBy(e=>e.User.FirstName),
            asNoTracking: true);
    }

    public async Task<Trainer?> GetByNameAsync(string name)
    {
        return await _repository.Get(
            selector: x => x,
            predicate: x => x.User.FirstName == name,
            include: x=>x.Include(e=>e.User)
        );
    }
}
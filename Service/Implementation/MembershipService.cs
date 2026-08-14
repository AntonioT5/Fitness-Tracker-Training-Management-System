using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class MembershipService : IMembershipService
{
    private readonly IRepository<Membership> _repository;
    
    public MembershipService(IRepository<Membership> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<Membership>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x,
            include: x=>x.Include(e=>e.Gym)
                .Include(e=>e.Member).ThenInclude(e=>e.User));
        return result.ToList();
    }

    public async Task<Membership?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id,
            include: x=>x.Include(e=>e.Gym)
                .Include(e=>e.Member).ThenInclude(e=>e.User));
    }

    public async Task<Membership> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"Membership with id {id} not found");
        }
        return result;
    }

    public async Task<Membership> InsertAsync(MembershipDto membershipDto)
    {
        var membership = new Membership()
        {
            StartDate = membershipDto.StartDate,
            EndDate = membershipDto.EndDate,
            Price = membershipDto.Price,
            IsActive = membershipDto.IsActive,
            Type = membershipDto.Type,
            GymId = membershipDto.GymId,
            MemberId = membershipDto.MemberId
        };
        return await _repository.InsertAsync(membership);
    }

    public async Task<Membership> UpdateAsync(Guid id, MembershipDto membershipDto)
    {
        var result = await GetByIdNotNullAsync(id);
        result.StartDate = membershipDto.StartDate;
        result.EndDate = membershipDto.EndDate;
        result.Price = membershipDto.Price;
        result.IsActive = membershipDto.IsActive;
        result.Type = membershipDto.Type;
        result.GymId = membershipDto.GymId;
        result.MemberId = membershipDto.MemberId;
        return await _repository.UpdateAsync(result);
    }

    public async Task<Membership> DeleteAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(result);
    }
    
    public async Task<PaginatedResult<Membership>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x=>x.Include(e=>e.Gym)
                .Include(e=>e.Member).ThenInclude(e=>e.User),
            orderBy: x=>x.OrderBy(e=>e.StartDate),
            asNoTracking: true);
    }

    public async Task<Membership?> GetActiveByMemberIdAsync(Guid memberId)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.MemberId == memberId && x.IsActive && x.StartDate<=DateTime.UtcNow && x.EndDate>=DateTime.UtcNow
            );
    }
}
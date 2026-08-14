using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IMembershipService
{
    Task<List<Membership>> GetAllAsync();
    Task<Membership?> GetByIdAsync(Guid id);
    Task<Membership> GetByIdNotNullAsync(Guid id);
    Task<Membership> InsertAsync(MembershipDto membershipDto);
    Task<Membership> UpdateAsync(Guid id, MembershipDto membershipDto);
    Task<Membership> DeleteAsync(Guid id);
    
    public Task<PaginatedResult<Membership>> GetAllPagedAsync(int pageNumber, int pageSize);
    
    Task<Membership?> GetActiveByMemberIdAsync(Guid memberId);
}
using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IMemberService
{
    Task<List<Member>> GetAllAsync();
    Task<Member?> GetByIdAsync(Guid id);
    Task<Member> GetByIdNotNullAsync(Guid id);
    Task<Member> InsertAsync(MemberDto memberDto);
    Task<Member> UpdateAsync(Guid id, MemberDto memberDto);
    Task<Member> DeleteAsync(Guid id);
    
    public Task<PaginatedResult<Member>> GetAllPagedAsync(int pageNumber, int pageSize);

    Task<Member?> GetByEmailAsync(string email);

}
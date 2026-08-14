using Domain.Dto;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class MemberService : IMemberService
{
    private readonly IRepository<Member> _repository;
    private readonly UserManager<GymAppUser> _userManager;
    
    public MemberService(IRepository<Member> repository, UserManager<GymAppUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }
    
    public async Task<List<Member>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x,
            include: x=>x.Include(e=>e.User));
        return result.ToList();
    }

    public async Task<Member?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id,
            include: x=>x.Include(e=>e.User));
    }

    public async Task<Member> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"Member with id {id} not found");
        }
        return result;
    }

    public async Task<Member> InsertAsync(MemberDto memberDto)
    {
        var user = await _userManager.FindByIdAsync(memberDto.UserId.ToString());
        if (user == null)
            throw new InvalidOperationException($"User with id {memberDto.UserId} not found");
        
        var member = new Member()
        {
            UserId = memberDto.UserId,
            DateOfBirth = memberDto.DateOfBirth,
            Goal = memberDto.Goal
        };
        var insertedMember = await _repository.InsertAsync(member);

        return await GetByIdNotNullAsync(insertedMember.Id);
    }

    public async Task<Member> UpdateAsync(Guid id, MemberDto memberDto)
    {
        var user = await _userManager.FindByIdAsync(memberDto.UserId.ToString());
        if (user == null)
            throw new InvalidOperationException($"User with id {memberDto.UserId} not found");
        
        var result = await GetByIdNotNullAsync(id);
        result.UserId = memberDto.UserId;
        result.DateOfBirth = memberDto.DateOfBirth;
        result.Goal = memberDto.Goal;
        return await _repository.UpdateAsync(result);
    }

    public async Task<Member> DeleteAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(result);
    }
    
    public async Task<PaginatedResult<Member>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _repository.GetAllPagedAsync(
            selector: x => x,
            pageNumber: pageNumber,
            pageSize: pageSize,
            include: x=>x.Include(e=>e.User),
            orderBy: x=>x.OrderBy(e=>e.User.FirstName),
            asNoTracking: true);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await _repository.Get(
            selector: x => x,
            predicate: x => x.User.Email == email,
            include: x=>x.Include(e=>e.User)
        );
    }
}
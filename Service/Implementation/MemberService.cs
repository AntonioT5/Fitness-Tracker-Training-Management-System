using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class MemberService : IMemberService
{
    private readonly IRepository<Member> _repository;
    
    public MemberService(IRepository<Member> repository)
    {
        _repository = repository;
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
        var member = new Member()
        {
            UserId = memberDto.UserId,
            DateOfBirth = memberDto.DateOfBirth,
            Goal = memberDto.Goal
        };
        return await _repository.InsertAsync(member);
    }

    public async Task<Member> UpdateAsync(Guid id, MemberDto memberDto)
    {
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
}
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class MemberWorkoutPlanService : IMemberWorkoutPlanService
{
    private readonly IRepository<MemberWorkoutPlan> _repository;
    
    public MemberWorkoutPlanService(IRepository<MemberWorkoutPlan> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<MemberWorkoutPlan>> GetAllAsync()
    {
        var result = await _repository.GetAllAsync(x => x,
            include: x=>x.Include(e=>e.Member).ThenInclude(e=>e.User)
                .Include(e=>e.WorkoutPlan));
        return result.ToList();
    }

    public async Task<MemberWorkoutPlan?> GetByIdAsync(Guid id)
    {
        return await _repository.Get(selector: x => x,
            predicate: x => x.Id == id,
            include: x=>x.Include(e=>e.Member).ThenInclude(e=>e.User)
                .Include(e=>e.WorkoutPlan));
    }

    public async Task<MemberWorkoutPlan> GetByIdNotNullAsync(Guid id)
    {
        var result = await GetByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException($"MemberWorkoutPlan with id {id} not found");
        }
        return result;
    }

    public async Task<MemberWorkoutPlan> InsertAsync(MemberWorkoutPlanDto memberWorkoutPlanDto)
    {
        var result = new MemberWorkoutPlan()
        {
            AssignedDate = memberWorkoutPlanDto.AssignedDate,
            Status = memberWorkoutPlanDto.Status,
            MemberId = memberWorkoutPlanDto.MemberId,
            WorkoutPlanId = memberWorkoutPlanDto.WorkoutPlanId
        };
        return await _repository.InsertAsync(result);
    }

    public async Task<MemberWorkoutPlan> UpdateAsync(Guid id, MemberWorkoutPlanDto memberWorkoutPlanDto)
    {
        var result = await GetByIdNotNullAsync(id);
        result.AssignedDate = memberWorkoutPlanDto.AssignedDate;
        result.Status = memberWorkoutPlanDto.Status;
        result.MemberId = memberWorkoutPlanDto.MemberId;
        result.WorkoutPlanId = memberWorkoutPlanDto.WorkoutPlanId;
        return await _repository.UpdateAsync(result);
    }

    public async Task<MemberWorkoutPlan> DeleteAsync(Guid id)
    {
        var result = await GetByIdNotNullAsync(id);
        return await _repository.DeleteAsync(result);
    }
}
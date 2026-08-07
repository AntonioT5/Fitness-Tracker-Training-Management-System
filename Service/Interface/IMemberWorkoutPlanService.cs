using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IMemberWorkoutPlanService
{
    Task<List<MemberWorkoutPlan>> GetAllAsync();
    Task<MemberWorkoutPlan?> GetByIdAsync(Guid id);
    Task<MemberWorkoutPlan> GetByIdNotNullAsync(Guid id);
    Task<MemberWorkoutPlan> InsertAsync(MemberWorkoutPlanDto memberWorkoutPlanDto);
    Task<MemberWorkoutPlan> UpdateAsync(Guid id, MemberWorkoutPlanDto memberWorkoutPlanDto);
    Task<MemberWorkoutPlan> DeleteAsync(Guid id);
}
using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class MemberWorkoutPlanExtensions
{
    public static MemberWorkoutPlanResponse? ToResponse(this MemberWorkoutPlan memberWorkoutPlan)
    {
        return new MemberWorkoutPlanResponse(
            memberWorkoutPlan.Id,
            memberWorkoutPlan.AssignedDate,
            memberWorkoutPlan.Status,
            memberWorkoutPlan.Member?.User?.FirstName,
            memberWorkoutPlan.WorkoutPlan?.Name
        );
    }

    public static List<MemberWorkoutPlanResponse?> ToResponse(this List<MemberWorkoutPlan> memberWorkoutPlan)
    {
        return memberWorkoutPlan.Select(x => x.ToResponse()).ToList();
    }

    public static MemberWorkoutPlanDto ToDto(this MemberWorkoutPlanRequest memberWorkoutPlan)
    {
        return new MemberWorkoutPlanDto
        {
            AssignedDate = memberWorkoutPlan.AssignedDate,
            Status = memberWorkoutPlan.Status,
            MemberId = memberWorkoutPlan.MemberId,
            WorkoutPlanId = memberWorkoutPlan.WorkoutPlanId
        };
    }
}
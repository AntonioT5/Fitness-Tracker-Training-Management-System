using Domain.Enums;

namespace Domain.Dto;

public class MemberWorkoutPlanDto
{
    public DateTime AssignedDate { get; set; }
    public PlanStatus Status { get; set; }

    public Guid MemberId { get; set; }
    public Guid WorkoutPlanId { get; set; }
}
using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class MemberWorkoutPlan : BaseEntity
{
    public DateTime AssignedDate { get; set; }
    public PlanStatus Status { get; set; }

    public Guid MemberId { get; set; }
    public virtual Member Member { get; set; } = null!;
    
    public Guid WorkoutPlanId { get; set; }
    public virtual WorkoutPlan WorkoutPlan { get; set; } = null!;
}
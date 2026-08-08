using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Web.Request;

public record MemberWorkoutPlanRequest(
    [Required] DateTime AssignedDate,
    [Required] PlanStatus Status,
    [Required] Guid MemberId,
    [Required] Guid WorkoutPlanId);
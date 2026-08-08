using Domain.Enums;

namespace Web.Response;

public record MemberWorkoutPlanResponse(
    Guid Id,
    DateTime AssignedDate,
    PlanStatus Status,
    string? MemberUserFirstName,
    string? WorkoutPlanName);
namespace Web.Response;

public record WorkoutPlanResponse(
    Guid Id,
    string Name,
    string Description,
    int DurationWeeks,
    string? TrainerUserFirstName);
namespace Web.Response;

public record WorkoutSessionResponse(
    Guid Id,
    DateTime Date,
    int SetsCompleted,
    int RepsCompleted,
    decimal WeightUsedKg,
    int DurationMinutes,
    string? Notes,
    string? MemberUserFirstName,
    string? TrainerUserFirstName,
    string? ExerciseName);
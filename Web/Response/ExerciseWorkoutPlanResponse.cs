namespace Web.Response;

public record ExerciseWorkoutPlanResponse(
    Guid Id,
    int Sets,
    int Reps,
    string? ExerciseName,
    string? WorkoutPlanName);
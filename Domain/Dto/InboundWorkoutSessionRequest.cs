namespace Domain.Dto;

public record InboundWorkoutSessionRequest(
    string MemberEmail,
    string TrainerName,
    string ExerciseName,
    DateTime SessionDate,
    int SetsCompleted,
    int RepsCompleted,
    decimal WeightUsedKg,
    int DurationMinutes,
    string? Notes
    );
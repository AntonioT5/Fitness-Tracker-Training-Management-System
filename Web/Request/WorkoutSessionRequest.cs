using System.ComponentModel.DataAnnotations;

namespace Web.Request;

public record WorkoutSessionRequest(
    [Required] DateTime Date,
    [Required] int SetsCompleted,
    [Required] int RepsCompleted,
    [Required] decimal WeightUsedKg,
    [Required] int DurationMinutes,
    string? Notes,
    [Required] Guid MemberId,
    [Required] Guid TrainerId,
    [Required] Guid ExerciseId);
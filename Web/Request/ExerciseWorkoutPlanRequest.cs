using System.ComponentModel.DataAnnotations;

namespace Web.Request;

public record ExerciseWorkoutPlanRequest(
    [Required] int Sets,
    [Required] int Reps,
    [Required] Guid ExerciseId,
    [Required] Guid WorkoutPlanId);
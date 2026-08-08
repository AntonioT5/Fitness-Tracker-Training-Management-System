using System.ComponentModel.DataAnnotations;

namespace Web.Request;

public record WorkoutPlanRequest(
    [Required] string Name,
    [Required] string Description,
    [Required] int DurationWeeks,
    [Required] Guid TrainerId);
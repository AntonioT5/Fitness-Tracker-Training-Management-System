using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Web.Request;

public record ExerciseRequest(
    [Required] string Name,
    [Required] string MuscleGroup,
    [Required] DifficultyLevel Difficulty,
    [Required] string Equipment,
    [Required] string Description );
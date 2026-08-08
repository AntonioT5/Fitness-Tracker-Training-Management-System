using Domain.Enums;

namespace Web.Response;

public record ExerciseResponse(
    Guid Id,
    string Name,
    string MuscleGroup,
    DifficultyLevel Difficulty,
    string Equipment,
    string Description);
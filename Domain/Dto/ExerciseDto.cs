using Domain.Enums;

namespace Domain.Dto;

public class ExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public string Equipment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
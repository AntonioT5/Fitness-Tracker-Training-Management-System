namespace Domain.Dto;

public class WorkoutSessionImportDto
{
    public Guid MemberId { get; set; }
    public Guid TrainerId { get; set; }
    public Guid ExerciseId { get; set; }
    public DateTime Date { get; set; }
    public int SetsCompleted { get; set; }
    public int RepsCompleted { get; set; }
    public decimal WeightUsedKg { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
}
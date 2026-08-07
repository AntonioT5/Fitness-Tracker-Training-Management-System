namespace Domain.Dto;

public class WorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationWeeks  { get; set; }
    
    public Guid TrainerId { get; set; }
}
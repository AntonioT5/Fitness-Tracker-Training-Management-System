namespace Domain.Dto;

public class ExerciseWorkoutPlanDto
{
    public int Sets { get; set; }
    public int Reps { get; set; }
    
    public Guid ExerciseId { get; set; }
    public Guid WorkoutPlanId { get; set; }
}
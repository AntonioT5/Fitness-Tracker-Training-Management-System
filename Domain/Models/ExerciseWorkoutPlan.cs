using Domain.Common;

namespace Domain.Models;

public class ExerciseWorkoutPlan : BaseEntity
{
    public int Sets { get; set; }
    public int Reps { get; set; }
    
    public Guid? ExerciseId { get; set; }
    public virtual Exercise Exercise { get; set; } = null!;

    public Guid? WorkoutPlanId { get; set; }
    public virtual WorkoutPlan WorkoutPlan { get; set; } = null!;
}
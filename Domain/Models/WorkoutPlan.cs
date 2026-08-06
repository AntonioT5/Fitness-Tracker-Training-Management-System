using Domain.Common;

namespace Domain.Models;

public class WorkoutPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationWeeks  { get; set; }
    
    public Guid? TrainerId { get; set; }
    public virtual Trainer Trainer { get; set; } = null!;
    
    public virtual ICollection<MemberWorkoutPlan> MemberWorkoutPlans { get; set; } = new List<MemberWorkoutPlan>();
    public virtual ICollection<ExerciseWorkoutPlan> ExerciseWorkoutPlans { get; set; } = new List<ExerciseWorkoutPlan>();
}
using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class Exercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public string Equipment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public virtual ICollection<ExerciseWorkoutPlan> ExerciseWorkoutPlans { get; set; } = new List<ExerciseWorkoutPlan>();
    public virtual ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
}
using Domain.Common;

namespace Domain.Models;

public class Trainer : BaseEntity
{
    public required string UserId { get; set; }
    public virtual GymAppUser User { get; set; } = null!;

    public int YearsExperience { get; set; }
    
    public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
    public virtual ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
}
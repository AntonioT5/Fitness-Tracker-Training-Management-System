using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class Member : BaseEntity
{
    public string? UserId { get; set; }
    public virtual GymAppUser User { get; set; } = null!;
    
    public DateTime DateOfBirth { get; set; }
    public FitnessGoal Goal { get; set; }
    
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public virtual ICollection<MemberWorkoutPlan> MemberWorkoutPlans { get; set; } = new List<MemberWorkoutPlan>();
    public virtual ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
}
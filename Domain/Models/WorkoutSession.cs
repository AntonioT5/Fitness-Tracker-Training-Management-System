using Domain.Common;

namespace Domain.Models;

public class WorkoutSession : BaseAuditableEntity<GymAppUser>
{
    public DateTime Date { get; set; }
    public int SetsCompleted { get; set; }
    public int RepsCompleted { get; set; }
    public decimal WeightUsedKg { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
    
    public Guid MemberId { get; set; }
    public virtual Member Member { get; set; } = null!;

    public Guid TrainerId { get; set; }
    public virtual Trainer Trainer { get; set; } = null!;

    public Guid ExerciseId { get; set; }
    public virtual Exercise Exercise { get; set; } = null!;
}
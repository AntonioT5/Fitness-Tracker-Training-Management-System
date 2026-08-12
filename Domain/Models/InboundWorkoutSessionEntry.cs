using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class InboundWorkoutSessionEntry : BaseEntity
{
    public string RawPayload { get; set; } = string.Empty;
    public InboundWorkoutSessionStatus Status { get; set; }
    public Guid ApiClientId { get; set; }
    public virtual ApiClient ApiClient { get; set; } = null!;
    public DateTime ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? CreatedWorkoutSessionId { get; set; }
}
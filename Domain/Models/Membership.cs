using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public class Membership : BaseAuditableEntity<GymAppUser>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public MembershipType Type { get; set; }
    
    public Guid GymId { get; set; }
    public virtual Gym Gym { get; set; } = null!;
    
    public Guid MemberId { get; set; }
    public virtual Member Member { get; set; } = null!;
}
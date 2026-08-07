using Domain.Enums;

namespace Domain.Dto;

public class MembershipDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public MembershipType Type { get; set; }
    
    public Guid GymId { get; set; }
    public Guid MemberId { get; set; }
}
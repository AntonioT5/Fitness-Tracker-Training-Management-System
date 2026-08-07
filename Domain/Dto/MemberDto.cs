using Domain.Enums;

namespace Domain.Dto;

public class MemberDto
{
    public required string UserId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public FitnessGoal Goal { get; set; }
}
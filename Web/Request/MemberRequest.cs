using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Web.Request;

public record MemberRequest(
    [Required] string UserId,
    [Required] DateTime DateOfBirth,
    [Required] FitnessGoal Goal
);
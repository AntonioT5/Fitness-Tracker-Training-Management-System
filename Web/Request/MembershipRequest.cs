using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Web.Request;

public record MembershipRequest(
    [Required] DateTime StartDate,
    [Required] DateTime EndDate,
    [Required] decimal Price,
    [Required] bool IsActive,
    [Required] MembershipType Type,
    [Required] Guid GymId,
    [Required] Guid MemberId);
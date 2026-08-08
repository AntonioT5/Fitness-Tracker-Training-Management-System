using Domain.Enums;

namespace Web.Response;

public record MembershipResponse(
    Guid Id,
    DateTime StartDate,
    DateTime EndDate,
    decimal Price,
    bool IsActive,
    MembershipType Type,
    string? GymName,
    string? MemberUserFirstName);
using Domain.Enums;

namespace Web.Response;

public record MemberResponse(
    Guid Id,
    string UserFirstName,
    string UserLastName,
    DateTime DateOfBirth,
    FitnessGoal Goal);
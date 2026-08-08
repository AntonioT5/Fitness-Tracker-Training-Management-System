namespace Web.Response;

public record GymResponse(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    string Email);
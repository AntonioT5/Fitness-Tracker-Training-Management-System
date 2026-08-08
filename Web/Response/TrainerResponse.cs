namespace Web.Response;

public record TrainerResponse(
    Guid Id,
    string UserFirstName,
    string UserLastName,
    int YearsExperience);
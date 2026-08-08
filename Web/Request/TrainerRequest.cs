using System.ComponentModel.DataAnnotations;

namespace Web.Request;

public record TrainerRequest(
    [Required] string UserId,
    [Required] int YearsExperience);
using System.ComponentModel.DataAnnotations;

namespace Web.Request;

public record GymRequest(
    [Required] string Name,
    [Required] string Address,
    [Required] string Phone,
    [Required] string Email);
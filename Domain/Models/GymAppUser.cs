using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class GymAppUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
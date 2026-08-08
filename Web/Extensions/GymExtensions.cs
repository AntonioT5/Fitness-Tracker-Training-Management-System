using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class GymExtensions
{
    public static GymResponse? ToResponse(this Gym g)
    {
        return new GymResponse(
            g.Id,
            g.Name,
            g.Address,
            g.Phone,
            g.Email
        );
    }

    public static List<GymResponse?> ToResponse(this List<Gym> gym)
    {
        return gym.Select(x => x.ToResponse()).ToList();
    }

    public static GymDto ToDto(this GymRequest gym)
    {
        return new GymDto
        {
            Name = gym.Name,
            Address = gym.Address,
            Phone =  gym.Phone,
            Email = gym.Email
        };
    }
}
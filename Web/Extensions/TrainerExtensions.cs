using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class TrainerExtensions
{
    public static TrainerResponse? ToResponse(this Trainer trainer)
    {
        return new TrainerResponse(
            trainer.Id,
            trainer.User?.FirstName,
            trainer.User?.LastName,
            trainer.YearsExperience
        );
    }

    public static List<TrainerResponse?> ToResponse(this List<Trainer> e)
    {
        return e.Select(x => x.ToResponse()).ToList();
    }

    public static TrainerDto ToDto(this TrainerRequest trainer)
    {
        return new TrainerDto
        {
            UserId = trainer.UserId,
            YearsExperience =  trainer.YearsExperience
        };
    }
}
using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class ExerciseExtensions
{
    public static ExerciseResponse? ToResponse(this Exercise exercise)
    {
        return new ExerciseResponse(
            exercise.Id,
            exercise.Name,
            exercise.MuscleGroup,
            exercise.Difficulty,
            exercise.Equipment,
            exercise.Description
        );
    }

    public static List<ExerciseResponse?> ToResponse(this List<Exercise> exercises)
    {
        return exercises.Select(x => x.ToResponse()).ToList();
    }

    public static ExerciseDto ToDto(this ExerciseRequest request)
    {
        return new ExerciseDto
        {
            Name = request.Name,
            MuscleGroup = request.MuscleGroup,
            Difficulty = request.Difficulty,
            Equipment = request.Equipment,
            Description = request.Description
        };
    }
}
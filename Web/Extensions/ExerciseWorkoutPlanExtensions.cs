using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class ExerciseWorkoutPlanExtensions
{
    public static ExerciseWorkoutPlanResponse? ToResponse(this ExerciseWorkoutPlan exerciseWorkoutPlan)
    {
        return new ExerciseWorkoutPlanResponse(
            exerciseWorkoutPlan.Id,
            exerciseWorkoutPlan.Sets,
            exerciseWorkoutPlan.Reps,
            exerciseWorkoutPlan.Exercise?.Name,
            exerciseWorkoutPlan.WorkoutPlan?.Name
            );
    }

    public static List<ExerciseWorkoutPlanResponse?> ToResponse(this List<ExerciseWorkoutPlan> e)
    {
        return e.Select(x => x.ToResponse()).ToList();
    }

    public static ExerciseWorkoutPlanDto ToDto(this ExerciseWorkoutPlanRequest e)
    {
        return new ExerciseWorkoutPlanDto
        {
            Sets = e.Sets,
            Reps = e.Reps,
            ExerciseId =  e.ExerciseId,
            WorkoutPlanId = e.WorkoutPlanId
        };
    }
}
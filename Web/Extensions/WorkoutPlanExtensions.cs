using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class WorkoutPlanExtensions
{
    public static WorkoutPlanResponse? ToResponse(this WorkoutPlan workoutPlan)
    {
        return new WorkoutPlanResponse(
            workoutPlan.Id,
            workoutPlan.Name,
            workoutPlan.Description,
            workoutPlan.DurationWeeks,
            workoutPlan.Trainer?.User?.FirstName
        );
    }

    public static List<WorkoutPlanResponse?> ToResponse(this List<WorkoutPlan> e)
    {
        return e.Select(x => x.ToResponse()).ToList();
    }

    public static WorkoutPlanDto ToDto(this WorkoutPlanRequest workoutPlan)
    {
        return new WorkoutPlanDto
        {
            Name = workoutPlan.Name,
            Description = workoutPlan.Description,
            DurationWeeks = workoutPlan.DurationWeeks,
            TrainerId = workoutPlan.TrainerId
        };
    }
}
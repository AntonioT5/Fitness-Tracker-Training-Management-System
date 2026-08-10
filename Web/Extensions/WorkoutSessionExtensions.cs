using Domain.Dto;
using Domain.Models;
using Web.Request;
using Web.Response;

namespace Web.Extensions;

public static class WorkoutSessionExtensions
{
    public static WorkoutSessionResponse? ToResponse(this WorkoutSession workoutSession)
    {
        return new WorkoutSessionResponse(
            workoutSession.Id,
            workoutSession.Date,
            workoutSession.SetsCompleted,
            workoutSession.RepsCompleted,
            workoutSession.WeightUsedKg,
            workoutSession.DurationMinutes,
            workoutSession.Notes,
            workoutSession.Member?.User?.FirstName,
            workoutSession.Trainer?.User?.FirstName,
            workoutSession.Exercise?.Name
        );
    }

    public static List<WorkoutSessionResponse?> ToResponse(this List<WorkoutSession> workoutSession)
    {
        return workoutSession.Select(x => x.ToResponse()).ToList();
    }

    public static WorkoutSessionDto ToDto(this WorkoutSessionRequest workoutSession)
    {
        return new WorkoutSessionDto
        {
            Date = DateTime.SpecifyKind(workoutSession.Date, DateTimeKind.Utc),
            SetsCompleted = workoutSession.SetsCompleted,
            RepsCompleted = workoutSession.RepsCompleted,
            WeightUsedKg = workoutSession.WeightUsedKg,
            DurationMinutes = workoutSession.DurationMinutes,
            Notes = workoutSession.Notes,
            MemberId = workoutSession.MemberId,
            TrainerId = workoutSession.TrainerId,
            ExerciseId = workoutSession.ExerciseId
        };
    }
}
using Domain.Dto;
using Domain.WgerApiResponse;

namespace Service.Interface;

public interface IWgerApiClient
{
    Task<List<ExerciseWgerDto>> GetAllExercisesAsync();
}
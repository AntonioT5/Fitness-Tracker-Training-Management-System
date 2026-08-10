using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class ExerciseWorkoutPlanMapper
{
    private readonly IExerciseWorkoutPlanService _service;

    public ExerciseWorkoutPlanMapper(IExerciseWorkoutPlanService service)
    {
        _service = service;
    }

    public async Task<List<ExerciseWorkoutPlanResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<ExerciseWorkoutPlanResponse?> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdNotNullAsync(id);
            return result.ToResponse();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
    
    public async Task<PaginatedResponse<ExerciseWorkoutPlanResponse?>> PaginatedGetAllAsync(PaginateRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(e => e.ToResponse());
    }

    public async Task<ExerciseWorkoutPlanResponse?> InsertAsync(ExerciseWorkoutPlanRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }
    
    public async Task<ExerciseWorkoutPlanResponse?> UpdateAsync(Guid id, ExerciseWorkoutPlanRequest request)
    {
        var dto = request.ToDto();
        try
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.ToResponse();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
    
    public async Task<ExerciseWorkoutPlanResponse?> DeleteAsync(Guid id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            return result.ToResponse();
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
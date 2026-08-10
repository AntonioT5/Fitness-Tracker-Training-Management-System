using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class WorkoutPlanMapper
{
    private readonly IWorkoutPlanService _service;

    public WorkoutPlanMapper(IWorkoutPlanService service)
    {
        _service = service;
    }

    public async Task<List<WorkoutPlanResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<WorkoutPlanResponse?> GetById(Guid id)
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
    
    public async Task<PaginatedResponse<WorkoutPlanResponse?>> PaginatedGetAllAsync(PaginateRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(e => e.ToResponse());
    }

    public async Task<WorkoutPlanResponse?> InsertAsync(WorkoutPlanRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }
    
    public async Task<WorkoutPlanResponse?> UpdateAsync(Guid id, WorkoutPlanRequest request)
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
    
    public async Task<WorkoutPlanResponse?> DeleteAsync(Guid id)
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
using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class WorkoutSessionMapper
{
    private readonly IWorkoutSessionService _service;
    
    public WorkoutSessionMapper(IWorkoutSessionService service)
    {
        _service = service;
    }

    public async Task<List<WorkoutSessionResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<WorkoutSessionResponse?> GetById(Guid id)
    {
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<WorkoutSessionResponse?> InsertAsync(WorkoutSessionRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }
    
    public async Task<WorkoutSessionResponse?> UpdateAsync(Guid id, WorkoutSessionRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }
    
    public async Task<WorkoutSessionResponse?> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result.ToResponse();
    }
}
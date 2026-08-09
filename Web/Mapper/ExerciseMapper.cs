using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class ExerciseMapper
{
    private readonly IExerciseService _service;
    
    public ExerciseMapper(IExerciseService service)
    {
        _service = service;
    }

    public async Task<ExerciseResponse?> GetById(Guid id)
    {
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }
    
    public async Task<List<ExerciseResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }
    
    public async Task<PaginatedResponse<ExerciseResponse?>> PaginatedGetAllAsync(PaginateRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(e => e.ToResponse());
    }
    
    public async Task<ExerciseResponse?> InsertAsync(ExerciseRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }

    public async Task<ExerciseResponse?> UpdateAsync(Guid id, ExerciseRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }
    
    public async Task<ExerciseResponse?> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result.ToResponse();
    }
    
}
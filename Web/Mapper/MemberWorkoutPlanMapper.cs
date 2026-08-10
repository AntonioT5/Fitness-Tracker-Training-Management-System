using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class MemberWorkoutPlanMapper
{
    private readonly IMemberWorkoutPlanService _service;

    public MemberWorkoutPlanMapper(IMemberWorkoutPlanService service)
    {
        _service = service;
    }

    public async Task<List<MemberWorkoutPlanResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<MemberWorkoutPlanResponse?> GetById(Guid id)
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
    
    public async Task<PaginatedResponse<MemberWorkoutPlanResponse?>> PaginatedGetAllAsync(PaginateRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(e => e.ToResponse());
    }

    public async Task<MemberWorkoutPlanResponse?> InsertAsync(MemberWorkoutPlanRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }
    
    public async Task<MemberWorkoutPlanResponse?> UpdateAsync(Guid id, MemberWorkoutPlanRequest request)
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
    
    public async Task<MemberWorkoutPlanResponse?> DeleteAsync(Guid id)
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
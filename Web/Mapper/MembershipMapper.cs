using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class MembershipMapper
{
    private readonly IMembershipService _service;

    public MembershipMapper(IMembershipService service)
    {
        _service = service;
    }

    public async Task<List<MembershipResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<MembershipResponse?> GetById(Guid id)
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
    
    public async Task<PaginatedResponse<MembershipResponse?>> PaginatedGetAllAsync(PaginateRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(e => e.ToResponse());
    }

    public async Task<MembershipResponse?> InsertAsync(MembershipRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }
    
    public async Task<MembershipResponse?> UpdateAsync(Guid id, MembershipRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }
    
    public async Task<MembershipResponse?> DeleteAsync(Guid id)
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
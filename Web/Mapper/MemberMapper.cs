using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class MemberMapper
{
    private readonly IMemberService _service;

    public MemberMapper(IMemberService service)
    {
        _service = service;
    }

    public async Task<List<MemberResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<MemberResponse?> GetById(Guid id)
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
    
    public async Task<PaginatedResponse<MemberResponse?>> PaginatedGetAllAsync(PaginateRequest request)
    {
        var result = await _service.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return result.ToPaginatedResponse(e => e.ToResponse());
    }

    public async Task<MemberResponse?> InsertAsync(MemberRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }
    
    public async Task<MemberResponse?> UpdateAsync(Guid id, MemberRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }
    
    public async Task<MemberResponse?> DeleteAsync(Guid id)
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
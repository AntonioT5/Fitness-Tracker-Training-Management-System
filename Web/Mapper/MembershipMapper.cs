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
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
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
        var result = await _service.DeleteAsync(id);
        return result.ToResponse();
    }
}
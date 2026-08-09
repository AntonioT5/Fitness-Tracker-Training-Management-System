using Service.Interface;
using Web.Extensions;
using Web.Request;
using Web.Response;

namespace Web.Mapper;

public class TrainerMapper
{
    private readonly ITrainerService _service;

    public TrainerMapper(ITrainerService service)
    {
        _service = service;
    }

    public async Task<List<TrainerResponse?>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.ToResponse();
    }

    public async Task<TrainerResponse?> GetById(Guid id)
    {
        var result = await _service.GetByIdNotNullAsync(id);
        return result.ToResponse();
    }

    public async Task<TrainerResponse?> InsertAsync(TrainerRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.InsertAsync(dto);
        return result.ToResponse();
    }
    
    public async Task<TrainerResponse?> UpdateAsync(Guid id, TrainerRequest request)
    {
        var dto = request.ToDto();
        var result = await _service.UpdateAsync(id, dto);
        return result.ToResponse();
    }
    
    public async Task<TrainerResponse?> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return result.ToResponse();
    }
}
using Domain.Enums;
using Domain.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class InboundWorkoutSessionEntryService : IInboundWorkoutSessionEntryService
{
    private readonly IRepository<InboundWorkoutSessionEntry> _repository;

    public InboundWorkoutSessionEntryService(IRepository<InboundWorkoutSessionEntry> repository)
    {
        _repository = repository;
    }

    public async Task<InboundWorkoutSessionEntry> CreateAsync(string rawPayload, Guid apiClientId)
    {
        var result = new InboundWorkoutSessionEntry
        {
            RawPayload = rawPayload,
            ApiClientId = apiClientId,
            ReceivedAt = DateTime.UtcNow,
            Status = InboundWorkoutSessionStatus.Pending
        };
        
        return await _repository.InsertAsync(result);
    }

    public async Task<InboundWorkoutSessionEntry?> GetByIdNotNull(Guid id)
    {
        try
        {
            var result = await _repository.Get(selector: x => x,
                predicate: x => x.Id == id);
            return result;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
using Domain.Models;

namespace Service.Interface;

public interface IInboundWorkoutSessionEntryService
{
    Task<InboundWorkoutSessionEntry> CreateAsync(string rawPayload, Guid apiClientId);
    Task<InboundWorkoutSessionEntry?> GetByIdNotNull(Guid id);
}
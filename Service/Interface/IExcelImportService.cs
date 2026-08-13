using Domain.Dto;

namespace Service.Interface;

public interface IExcelImportService
{
    public Task<ImportResult<WorkoutSessionImportDto>> ImportEventsAsync(Stream fileStream);
}
using Domain.Dto;
using Domain.Models;

namespace Service.Interface;

public interface IExcelImportService
{
    Task<ImportResult<WorkoutSessionImportDto>> ImportEventsAsync(Stream fileStream);
}
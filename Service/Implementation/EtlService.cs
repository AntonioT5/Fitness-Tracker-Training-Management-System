using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class EtlService : IEtlService
{
    private readonly IWgerApiClient _wgerApiClient;
    private readonly ILogger<EtlService> _logger;
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IRepository<EtlSyncLog> _etlRepository;

    public EtlService(IWgerApiClient wgerApiClient, ILogger<EtlService> logger, IRepository<Exercise> exerciseRepository, IRepository<EtlSyncLog> etlRepository)
    {
        _wgerApiClient = wgerApiClient;
        _logger = logger;
        _exerciseRepository = exerciseRepository;
        _etlRepository = etlRepository;
    }

    public async Task SyncAllAsync()
    {
        var syncLog = new EtlSyncLog
        {
            JobName = "ExerciseSync",
            StartedAt = DateTime.UtcNow
        };

        try
        {
            _logger.LogInformation("Starting Wger Exercise ETL");
            
            var exercises = await _wgerApiClient.GetAllExercisesAsync();
            
            _logger.LogInformation("Extracted and transformed {Count} exercises", exercises.Count);

            var processedCount = 0;
            foreach (var dto in exercises)
            {
                var existing = await _exerciseRepository.Get(
                    x => x,
                    predicate: x => x.Id == dto.ExternalId);

                if (existing == null)
                {
                    await _exerciseRepository.InsertAsync(new Exercise
                    {
                        Id = dto.ExternalId,
                        Name = dto.Name,
                        Description = dto.Description,
                        MuscleGroup = dto.MuscleGroup,
                        Equipment = dto.Equipment,
                        Difficulty = DifficultyLevel.Beginner
                    });
                }
                else
                {
                    existing.Name = dto.Name;
                    existing.Description = dto.Description;
                    existing.MuscleGroup = dto.MuscleGroup;
                    existing.Equipment = dto.Equipment;
                    await _exerciseRepository.UpdateAsync(existing);
                }
                processedCount++;
            }
            
            _logger.LogInformation("Successfully loaded {Count} exercises", processedCount);
            
            syncLog.Success = true;
            syncLog.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation("Legacy DB ETL finished successfully at {date}", syncLog.CompletedAt);
        }
        catch (Exception ex)
        {
            syncLog.Success = false;
            syncLog.ErrorMessage = ex.Message;
            syncLog.CompletedAt = DateTime.UtcNow;
            _logger.LogError(ex, "An error occured during the ETL process...");
        }
        finally
        {
            await _etlRepository.InsertAsync(syncLog);
        }
    }

}
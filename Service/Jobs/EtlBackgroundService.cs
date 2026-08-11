using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Implementation;
using Service.Interface;

namespace Service.Jobs;

public class EtlBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EtlBackgroundService> _logger;

    public EtlBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<EtlBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IEtlService>();

            try
            {
                _logger.LogInformation("Starting ETL job");

                await service.SyncAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ETL job");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }

}
using Microsoft.Extensions.Logging;
using Quartz;
using Service.Implementation;

namespace Service.Jobs;

public class InboundProcessingJob : IJob
{
    private readonly InboundWorkoutSessionEntryProcessor _entryProcessor;
    private readonly ILogger<InboundProcessingJob> _logger;

    public InboundProcessingJob(InboundWorkoutSessionEntryProcessor entryProcessor, ILogger<InboundProcessingJob> logger)
    {
        _entryProcessor = entryProcessor;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _entryProcessor.ProcessPendingEntriesAsync();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Inbound processing job failed");
            throw new JobExecutionException(ex, refireImmediately: false);
        }
    }
}
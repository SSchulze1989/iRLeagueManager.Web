using Microsoft.Extensions.DependencyInjection;

namespace iRLeagueApiCore.Services.ResultService.Excecution;

internal sealed class ResultCalculationQueue : IResultCalculationQueue
{
    private readonly IServiceProvider serviceProvider;
    private readonly IBackgroundTaskQueue taskQueue;

    public ResultCalculationQueue(IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue)
    {
        this.serviceProvider = serviceProvider;
        this.taskQueue = taskQueue;
    }

    public async Task QueueEventResultAsync(long eventId)
    {
        var scope = serviceProvider.CreateScope();
        await taskQueue.QueueBackgroundWorkItemAsync(async cancellationToken =>
        {
            var resultCalculation = scope.ServiceProvider.GetRequiredService<ExecuteEventResultCalculation>();
            await resultCalculation.Execute(eventId, cancellationToken);
            scope.Dispose();
        });
    }
}

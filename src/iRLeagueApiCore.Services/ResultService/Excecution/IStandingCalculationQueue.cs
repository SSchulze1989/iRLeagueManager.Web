namespace iRLeagueApiCore.Services.ResultService.Excecution;

internal interface IStandingCalculationQueue
{
    public Task QueueStandingCalculationAsync(long eventId);
}

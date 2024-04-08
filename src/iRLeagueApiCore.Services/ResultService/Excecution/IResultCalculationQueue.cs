namespace iRLeagueApiCore.Services.ResultService.Excecution;

public interface IResultCalculationQueue
{
    public Task QueueEventResultAsync(long eventId);
}

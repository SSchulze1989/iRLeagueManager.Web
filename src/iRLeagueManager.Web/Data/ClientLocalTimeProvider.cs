namespace iRLeagueManager.Web.Data;

/// <summary>
/// Provide functions to convert time into the clients local time zone
/// </summary>
public class ClientLocalTimeProvider
{
    private readonly SharedStateService sharedState;

    public ClientLocalTimeProvider(SharedStateService sharedState)
    {
        this.sharedState = sharedState;
    }

    public TimeSpan LocalTimeOffset => sharedState.LocalTimeOffset;

    public DateTime Now => new DateTimeOffset(DateTime.UtcNow).ToOffset(LocalTimeOffset).Date;

    public DateTime ConvertToLocal(DateTime utcTime)
    {
        var localTimeOffset = new DateTimeOffset(utcTime, TimeSpan.Zero).ToOffset(LocalTimeOffset);
        return localTimeOffset.DateTime;
    }

    public DateTime ConvertToUtc(DateTime localTime)
    {
        var localTimeOffset = new DateTimeOffset(localTime, LocalTimeOffset);
        return localTimeOffset.UtcDateTime;
    }
}

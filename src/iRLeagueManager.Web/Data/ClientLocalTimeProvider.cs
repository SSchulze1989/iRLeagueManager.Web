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
    public TimeZoneInfo LocalTimeZone => sharedState.LocalTimeZone;

    public DateTime Now => ConvertToLocal(DateTime.UtcNow);

    public DateTime ConvertToLocal(DateTime utcTime)
    {
        //var localTimeOffset = new DateTimeOffset(utcTime, TimeSpan.Zero).ToOffset(LocalTimeOffset);
        //return localTimeOffset.DateTime;
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, LocalTimeZone);
    }

    public DateTime ConvertToUtc(DateTime localTime)
    {
        //var localTimeOffset = new DateTimeOffset(localTime, LocalTimeOffset);
        //return localTimeOffset.UtcDateTime;
        return TimeZoneInfo.ConvertTimeToUtc(localTime);
    }
}

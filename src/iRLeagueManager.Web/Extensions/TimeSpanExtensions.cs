namespace iRLeagueManager.Web.Extensions;

public static class TimeSpanExtensions
{
    public static DateTime AsDateTime(this TimeSpan timeSpan)
    {
        return DateTime.Today.Add(timeSpan);
    }

    public static string LapTimeString(this TimeSpan timeSpan)
    {
        return timeSpan.AsDateTime().ToString(@"m:ss.fff");
    }

    public static string ShortTimeString(this TimeSpan timeSpan)
    {
        if (timeSpan > TimeSpan.Zero)
        {
            return $"{timeSpan.AsDateTime():mm:ss}";
        }
        return $"-{timeSpan.Negate().AsDateTime():mm:ss}";
    }
}

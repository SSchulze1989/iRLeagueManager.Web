namespace iRLeagueManager.Web.Extensions
{
    public static class TimeSpanExtensions
    {
        public static DateTime AsDateTime(this TimeSpan timeSpan)
        {
            return new DateTime(timeSpan.Ticks);
        }

        public static string LapTimeString(this TimeSpan timeSpan)
        {
            return timeSpan.AsDateTime().ToString(@"m:ss.fff");
        }
    }
}

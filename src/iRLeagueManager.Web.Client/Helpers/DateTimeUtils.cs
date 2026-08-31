namespace iRLeagueManager.Web.Helpers;

internal static class DateTimeUtils
{
    public static string ToShortDateFormatted(this DateTime dateTime)
    {
        return dateTime.ToString("dd.MM.yy");
    }
}

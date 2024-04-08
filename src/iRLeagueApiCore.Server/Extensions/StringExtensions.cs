using System.Text.RegularExpressions;

namespace iRLeagueApiCore.Server.Extensions;

public static class StringExtensions
{
    public static string PadNumbers(this string input, int n = 10)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(n, '0'));
    }
}

using System.Text.RegularExpressions;

namespace iRLeagueManager.Web.Extensions;

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

    public static string RegexReplace(this string input, string pattern, string replacement)
    {
        return Regex.Replace(input, pattern, replacement);
    }

    public static string Capitalize(this string input)
    {
        return string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
    }
}

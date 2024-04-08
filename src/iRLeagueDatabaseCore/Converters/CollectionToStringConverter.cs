using System.Globalization;
using System.Linq.Expressions;

namespace iRLeagueDatabaseCore.Converters;

public class CollectionToStringConverter<T> : ValueConverter<ICollection<T>, string>
{
    private const char defaultDelimiter = ';';

    public CollectionToStringConverter(CultureInfo culture, char delimiter = ';') :
        base(ConvertToString(delimiter, culture), ConvertToArray(delimiter, culture))
    {
    }

    public CollectionToStringConverter() : this(CultureInfo.InvariantCulture, defaultDelimiter)
    {
    }

    public CollectionToStringConverter(char delimiter) : this(CultureInfo.InvariantCulture, delimiter)
    {
    }

    private static Expression<Func<ICollection<T>, string>> ConvertToString(char delimiter, CultureInfo culture) =>
        array => string.Join(delimiter, array.Select(x => Convert.ToString(x, culture)));

    private static Expression<Func<string, ICollection<T>>> ConvertToArray(char delimiter, CultureInfo culture) =>
        str => str.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => ChangeType(x, culture))
            .ToList();

    private static T ChangeType(string value, CultureInfo culture)
    {
        if (typeof(T).IsEnum)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        return (T)Convert.ChangeType(value, typeof(T), culture);
    }
}

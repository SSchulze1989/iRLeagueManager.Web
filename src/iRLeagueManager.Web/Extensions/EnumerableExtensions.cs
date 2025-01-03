namespace iRLeagueManager.Web.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : notnull
    {
        return enumerable.Where(x => x is not null).OfType<T>();
    }

    public static bool None<T>(this IEnumerable<T?> enumerable)
    {
        return enumerable.Any() == false;
    }

    public static bool None<T>(this IEnumerable<T?> enumerable, Func<T?, bool> predicate)
    {
        return enumerable.Any(predicate) == false;
    }

    public static T? MinOrDefault<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable.Any() == false)
        {
            return default;
        }
        return enumerable.Min();
    }

    /// <summary>
    /// Returs an enumerable with index for each item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="start">start index for the first element</param>
    /// <returns></returns>
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable, int start = 0)
    {
        return enumerable.Select((x, i) => (x, i + start));
    }
}

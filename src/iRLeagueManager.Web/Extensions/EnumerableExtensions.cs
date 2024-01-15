﻿namespace iRLeagueManager.Web.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : notnull
    {
        return enumerable.Where(x => x is not null).OfType<T>();
    }

    public static T? MinOrDefault<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable.Any() == false)
        {
            return default;
        }
        return enumerable.Min();
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((x, i) => (x, i));
    }
}

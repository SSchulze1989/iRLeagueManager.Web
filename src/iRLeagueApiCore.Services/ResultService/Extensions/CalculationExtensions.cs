namespace iRLeagueApiCore.Services.ResultService.Extensions;

public static class CalculationExtensions
{
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : struct
    {
        return enumerable.OfType<T>();
    }

    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey? key)
    {
        if (key == null)
        {
            return default;
        }
        dictionary.TryGetValue(key, out TValue? value);
        return value;
    }

    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Nullable<TKey> key) where TKey : struct
    {
        return GetOrDefault(dictionary, key);
    }

    public static IEnumerable<TValue> GetMultiple<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
    {
        return dictionary
            .Where(x => keys.Contains(x.Key))
            .Select(x => x.Value);
    }
}

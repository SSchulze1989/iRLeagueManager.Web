namespace iRLeagueApiCore.Mocking.Extensions;

public static class EnumeratorExtensions
{
    /// <summary>
    /// Returns <see cref="IEnumerator{T}.Current" and calls <see cref="IEnumerator{T}.MoveNext()"/>
    /// Resets the enumerator when at the end of sequence/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerator"></param>
    /// <returns>Current value</returns>
    public static T? Next<T>(this IEnumerator<T> enumerator)
    {
        if (enumerator.MoveNext())
        {
            return enumerator.Current;
        }
        return default(T?);
    }

    public static Func<T> CreateSequence<T>(this IEnumerable<T> enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        return () =>
        {
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }
            enumerator = enumerable.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        };
    }
}

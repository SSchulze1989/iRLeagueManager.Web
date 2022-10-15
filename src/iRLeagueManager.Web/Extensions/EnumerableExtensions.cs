namespace iRLeagueManager.Web.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : notnull
        {
            return enumerable.Where(x => x is not null).OfType<T>();
        }
    }
}

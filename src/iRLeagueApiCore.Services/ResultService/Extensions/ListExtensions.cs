namespace iRLeagueApiCore.Services.ResultService.Extensions;
public static class ListExtensions
{
    public static void Move<T>(this IList<T> list, int index, int offset)
    {
        int newIndex = index + offset;

        if (newIndex < 0)
        {
            newIndex = 0;
        }
        if (newIndex >= list.Count)
        {
            newIndex = list.Count - 1;
        }

        T entry = list[index];
        list.RemoveAt(index);
        list.Insert(newIndex, entry);
    }
}

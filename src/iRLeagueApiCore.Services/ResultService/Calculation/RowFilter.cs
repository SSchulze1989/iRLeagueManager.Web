namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal abstract class RowFilter<TRow>
{
    public abstract IEnumerable<T> FilterRows<T>(IEnumerable<T> rows) where T : TRow;

    public static RowFilter<TRow> Default() => new DefaultRowFilter<TRow>();
}

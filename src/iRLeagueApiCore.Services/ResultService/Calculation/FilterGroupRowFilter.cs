namespace iRLeagueApiCore.Services.ResultService.Calculation;
internal sealed class FilterGroupRowFilter<T> : RowFilter<T>
{
    private readonly IList<(FilterCombination combination, RowFilter<T> rowFilter)> filters;

    public IEnumerable<(FilterCombination combination, RowFilter<T> rowFilter)> GetFilters() => filters;

    public FilterGroupRowFilter()
    {
        filters = new List<(FilterCombination, RowFilter<T>)>();
    }

    public FilterGroupRowFilter(IEnumerable<(FilterCombination, RowFilter<T>)> filters)
    {
        this.filters = filters.ToList();
    }

    public override IEnumerable<TRow> FilterRows<TRow>(IEnumerable<TRow> rows)
    {
        var originalRows = rows.ToList();
        foreach(var (combination, filter) in filters)
        {
            rows = combination switch
            {
                FilterCombination.And => filter.FilterRows(rows),
                FilterCombination.Or => rows.Union(filter.FilterRows(originalRows)),
                _ => rows,
            };
        }
        return rows.OrderBy(x => originalRows.IndexOf(x));
    }
}

public enum FilterCombination
{
    And,
    Or,
}

namespace iRLeagueManager.Web.Data.CsvExporter;

internal sealed class CsvColumn<T> where T : notnull
{
    public string Header { get; } = string.Empty;
    public Func<T, int, string> ValueFunc { get; } = (x, _) => x?.ToString() ?? string.Empty;

    public CsvColumn(string header)
    {
        Header = header;
    }

    public CsvColumn(string header, Func<T, int, string> valueFunc)
    {
        Header = header;
        ValueFunc = valueFunc ?? throw new ArgumentNullException(nameof(valueFunc));
    }

    public CsvColumn(string header, Func<T, string> valueFunc) : this(header, (x, _) => valueFunc(x))
    {
    }
}

namespace iRLeagueManager.Web.Data.CsvExporter;

internal sealed class CsvColumn<T> where T : notnull
{
    public string Header { get; } = string.Empty;
    public Func<T, string> ValueFunc { get; } = x => x?.ToString() ?? string.Empty;

    public CsvColumn(string header)
    {
        Header = header;
    }

    public CsvColumn(string header, Func<T, string> valueFunc)
    {
        Header = header;
        ValueFunc = valueFunc ?? throw new ArgumentNullException(nameof(valueFunc));
    }
}

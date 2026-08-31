using iRLeagueManager.Web.Extensions;
using System.Text;

namespace iRLeagueManager.Web.Data.CsvExporter;

internal sealed class CsvGenerator<T> where T : notnull
{
    private readonly string delimiter = ";";

    public List<CsvColumn<T>> Columns { get; } = [];

    public CsvGenerator(string delimiter = ";")
    {
        this.delimiter = delimiter;
    }

    public CsvGenerator<T> AddColumn(string header)
    {
        return AddColumn(header, x => x?.ToString() ?? string.Empty);
    }

    public CsvGenerator<T> AddColumn(string header, Func<T, string> valueFunc)
    {
        if (string.IsNullOrEmpty(header))
        {
            throw new ArgumentException("Header cannot be null or empty", nameof(header));
        }
        if (valueFunc == null)
        {
            throw new ArgumentNullException(nameof(valueFunc));
        }
        Columns.Add(new CsvColumn<T>(header, valueFunc));
        return this;
    }

    public CsvGenerator<T> AddColumn(string header, Func<T, int, string> valueFunc)
    {
        if (string.IsNullOrEmpty(header))
        {
            throw new ArgumentException("Header cannot be null or empty", nameof(header));
        }
        if (valueFunc == null)
        {
            throw new ArgumentNullException(nameof(valueFunc));
        }
        Columns.Add(new CsvColumn<T>(header, valueFunc));
        return this;
    }

    public string GenerateHeader()
    {
        return string.Join(delimiter, Columns.Select(c => c.Header));
    }

    public string GenerateRow(T item, int rowIndex)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        return string.Join(delimiter, Columns.Select(c => c.ValueFunc(item, rowIndex)));
    }

    public string GenerateCsv(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine(GenerateHeader());
        foreach (var (item, index) in items.WithIndex())
        {
            csvBuilder.AppendLine(GenerateRow(item, index));
        }
        return csvBuilder.ToString();
    }
}

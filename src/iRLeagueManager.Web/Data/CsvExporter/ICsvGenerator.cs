namespace iRLeagueManager.Web.Data.CsvExporter;

public interface ICsvGenerator<TData>
{
    public string GetName();

    public string ExportCsv(TData data);
}

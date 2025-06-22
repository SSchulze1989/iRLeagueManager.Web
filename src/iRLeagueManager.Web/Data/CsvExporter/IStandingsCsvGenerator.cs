using iRLeagueApiCore.Common.Models.Standings;

namespace iRLeagueManager.Web.Data.CsvExporter;

public interface IStandingsCsvGenerator
{
    public string ExportCsv(StandingsModel standings);
}

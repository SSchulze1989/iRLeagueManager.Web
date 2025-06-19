using iRLeagueApiCore.Common.Models;

namespace iRLeagueManager.Web.Data.CsvExporter;

public interface ITeamListCsvGenerator : ICsvGenerator
{
    public string ExportCsv(IEnumerable<TeamModel> teams);
}

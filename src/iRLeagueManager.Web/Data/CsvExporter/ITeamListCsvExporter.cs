using iRLeagueApiCore.Common.Models;

namespace iRLeagueManager.Web.Data.CsvExporter;

public interface ITeamListCsvGenerator : ICsvGenerator<IEnumerable<TeamModel>>
{
}

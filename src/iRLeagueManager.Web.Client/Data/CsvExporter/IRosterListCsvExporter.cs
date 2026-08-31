using iRLeagueApiCore.Common.Models.Rosters;

namespace iRLeagueManager.Web.Data.CsvExporter;

public interface IRosterListCsvExporter : ICsvGenerator<IEnumerable<RosterInfoModel>>
{
}

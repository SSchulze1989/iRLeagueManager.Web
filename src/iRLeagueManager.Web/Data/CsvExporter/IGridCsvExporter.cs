using iRLeagueApiCore.Common.Models.Rosters;

namespace iRLeagueManager.Web.Data.CsvExporter;

public interface IGridCsvExporter : ICsvGenerator<IEnumerable<RosterModel>>
{
}

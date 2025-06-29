using iRLeagueApiCore.Common.Models;

namespace iRLeagueManager.Web.Data.CsvExporter;

public interface IMemberListCsvGenerator : ICsvGenerator<IEnumerable<MemberModel>>
{
}

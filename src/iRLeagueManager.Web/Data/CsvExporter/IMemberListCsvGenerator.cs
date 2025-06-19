using iRLeagueApiCore.Common.Models;

namespace iRLeagueManager.Web.Data.CsvExporter;

public interface IMemberListCsvGenerator : ICsvGenerator
{
    public string ExportCsv(IEnumerable<MemberModel> members);
}

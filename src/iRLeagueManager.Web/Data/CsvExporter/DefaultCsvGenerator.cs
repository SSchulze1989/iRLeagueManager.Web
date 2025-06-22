using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueApiCore.Common.Models.Standings;

namespace iRLeagueManager.Web.Data.CsvExporter;

public class DefaultCsvGenerator : IMemberListCsvGenerator, ITeamListCsvGenerator, IStandingsCsvGenerator
{
    public string GetName()
    {
        return "Default CSV";
    }

    string IMemberListCsvGenerator.ExportCsv(IEnumerable<MemberModel> members)
    {
        var generator = new CsvGenerator<MemberModel>()
            .AddColumn("MemberId", x => x.MemberId.ToString())
            .AddColumn("Firstname", x => x.Firstname)
            .AddColumn("Lastname", x => x.Lastname)
            .AddColumn("IRacingId", x => x.IRacingId)
            .AddColumn("DiscordId", x => x.DiscordId ?? string.Empty)
            .AddColumn("TeamId", x => x.TeamId?.ToString() ?? string.Empty)
            .AddColumn("TeamName", x => x.TeamName ?? string.Empty)
            .AddColumn("Number", x => x.Number ?? string.Empty);
        foreach (var column in members.SelectMany(x => x.Profile.Keys).Distinct().ToList())
        {
            generator.AddColumn(column, x => x.Profile.GetValueOrDefault(column) ?? string.Empty);
        }
        return generator.GenerateCsv(members);
    }

    string ITeamListCsvGenerator.ExportCsv(IEnumerable<TeamModel> teams)
    {
        var generator = new CsvGenerator<TeamModel>()
            .AddColumn("TeamId", x => x.TeamId.ToString())
            .AddColumn("Name", x => x.Name ?? string.Empty)
            .AddColumn("IRacingTeamId", x => x.IRacingTeamId?.ToString() ?? string.Empty)
            .AddColumn("Profile", x => x.Profile ?? string.Empty)
            .AddColumn("TeamColor", x => x.TeamColor ?? string.Empty)
            .AddColumn("TeamHomepage", x => x.TeamHomepage ?? string.Empty);
        return generator.GenerateCsv(teams);
    }

    string IStandingsCsvGenerator.ExportCsv(StandingsModel standings)
    {
        var generator = new CsvGenerator<StandingRowModel>()
            .AddColumn("Position", x => x.Position.ToString())
            .AddColumn("MemberId", x => x.MemberId?.ToString() ?? string.Empty)
            .AddColumn("Firstname", x => x.Firstname)
            .AddColumn("Lastname", x => x.Lastname)
            .AddColumn("TeamName", x => x.TeamName ?? string.Empty)
            .AddColumn("TotalPoints", x => x.TotalPoints.ToString())
            .AddColumn("Races", x => x.Races.ToString())
            .AddColumn("Wins", x => x.Wins.ToString());
        return generator.GenerateCsv(standings.StandingRows);
    }
}

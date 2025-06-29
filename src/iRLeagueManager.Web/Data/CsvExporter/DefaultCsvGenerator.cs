using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueApiCore.Common.Models.Standings;

namespace iRLeagueManager.Web.Data.CsvExporter;

public class DefaultCsvGenerator : IMemberListCsvGenerator, ITeamListCsvGenerator, IStandingsCsvGenerator, IGridCsvExporter, IRosterListCsvExporter
{
    public string GetName()
    {
        return "Default CSV";
    }

    string ICsvGenerator<IEnumerable<MemberModel>>.ExportCsv(IEnumerable<MemberModel> members)
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

    string ICsvGenerator<IEnumerable<TeamModel>>.ExportCsv(IEnumerable<TeamModel> teams)
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

    string ICsvGenerator<StandingsModel>.ExportCsv(StandingsModel standings)
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

    private record GridRowModel(RosterModel Roster, RosterMemberModel Entry);

    string ICsvGenerator<IEnumerable<RosterModel>>.ExportCsv(IEnumerable<RosterModel> data)
    {
        var rows = data.SelectMany(roster => roster.RosterEntries.Select(entry => new GridRowModel(roster, entry))).ToList();
        var generator = new CsvGenerator<GridRowModel>()
            .AddColumn("RosterId", x => x.Roster.RosterId.ToString())
            .AddColumn("RosterName", x => x.Roster.Name ?? string.Empty)
            .AddColumn("MemberId", x => x.Entry.MemberId.ToString())
            .AddColumn("Firstname", x => x.Entry.Member.Firstname)
            .AddColumn("Lastname", x => x.Entry.Member.Lastname)
            .AddColumn("TeamId", x => x.Entry.TeamId?.ToString() ?? string.Empty)
            .AddColumn("TeamName", x => x.Entry.TeamName ?? string.Empty)
            .AddColumn("Number", x => x.Entry.Member.Number ?? string.Empty);
        return generator.GenerateCsv(rows);
    }

    string ICsvGenerator<IEnumerable<RosterInfoModel>>.ExportCsv(IEnumerable<RosterInfoModel> data)
    {
        var generator = new CsvGenerator<RosterInfoModel>()
            .AddColumn("RosterId", x => x.RosterId.ToString())
            .AddColumn("Name", x => x.Name)
            .AddColumn("Description", x => x.Description ?? string.Empty)
            .AddColumn("EntryCount", x => x.EntryCount.ToString());
        return generator.GenerateCsv(data);
    }
}

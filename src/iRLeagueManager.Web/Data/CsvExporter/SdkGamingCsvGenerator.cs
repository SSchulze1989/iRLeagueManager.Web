using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Rosters;
using iRLeagueApiCore.Common.Models.Standings;
using System.Drawing;

namespace iRLeagueManager.Web.Data.CsvExporter;

public class SdkGamingCsvGenerator : IMemberListCsvGenerator, ITeamListCsvGenerator, IStandingsCsvGenerator, IGridCsvExporter, IRosterListCsvExporter
{
    private static string ConvertToSdkColor(string color, bool hasAlphaValue = false)
    {
        if (string.IsNullOrEmpty(color))
        {
            return "Transparent";
        }
        if (color.StartsWith("#"))
        {
            return $"{(hasAlphaValue ? "ff" : "")}{color.Substring(1)}";
        }
        return color;
    }

    public string GetName()
    {
        return "SDK Gaming CSV";
    }

    string ICsvGenerator<IEnumerable<MemberModel>>.ExportCsv(IEnumerable<MemberModel> members)
    {
        var generator = new CsvGenerator<MemberModel>(",")
            .AddColumn("iRacing name", x => $"{x.Firstname} {x.Lastname}".Trim())
            .AddColumn("iRacing ID", x => x.IRacingId ?? string.Empty)
            .AddColumn("Multicar team background color", x => "Transparent")
            .AddColumn("Multicar team text color", x => "Transparent")
            .AddColumn("Multicar team logo url", x => x.Profile.TryGetValue("Multicar team logo url", out var info) ? info : string.Empty)
            .AddColumn("iRacing car color override", x => "Transparent")
            .AddColumn("iRacing car number color override", x => "Transparent")
            .AddColumn("First name override", x => x.Firstname)
            .AddColumn("Last name override", x => x.Lastname)
            .AddColumn("Suffix override", x => x.Profile.TryGetValue("Suffix override", out var info) ? info : string.Empty)
            .AddColumn("Initials override", x => x.Profile.TryGetValue("Initials override", out var info) ? info : string.Empty)
            .AddColumn("iRacing team name override", x => x.TeamName)
            .AddColumn("Multicar team name", x => x.TeamName)
            .AddColumn("Highlight", x => "None")
            .AddColumn("Club name override", x => "None")
            .AddColumn("Photo URL", x => x.Profile.TryGetValue("Photo URL", out var info) ? info : string.Empty)
            .AddColumn("Number URL", x => x.Profile.TryGetValue("Number URL", out var info) ? info : string.Empty)
            .AddColumn("Car Url", x => x.Profile.TryGetValue("Car Url", out var info) ? info : string.Empty)
            .AddColumn("Class 1", x => "None")
            .AddColumn("Class 2", x => "None")
            .AddColumn("Class 3", x => "None")
            .AddColumn("Birth date", x => x.Profile.TryGetValue("Birth date", out var info) ? info : string.Empty)
            .AddColumn("Home town", x => x.Profile.TryGetValue("Home town", out var info) ? info : string.Empty)
            .AddColumn("Driver header", x => x.Profile.TryGetValue("Driver header", out var info) ? info : string.Empty)
            .AddColumn("Driver information", x => x.Profile.TryGetValue("Driver information", out var info) ? info : string.Empty);
        return generator.GenerateCsv(members);
    }

    string ICsvGenerator<IEnumerable<TeamModel>>.ExportCsv(IEnumerable<TeamModel> teams)
    {
        var generator = new CsvGenerator<TeamModel>(",")
            .AddColumn("Name", x => x.Name ?? string.Empty)
            .AddColumn("Background color", x => ConvertToSdkColor(x.TeamColor, hasAlphaValue: true))
            .AddColumn("Text color", x => "Transparent")
            .AddColumn("Logo URL", x => string.Empty)
            .AddColumn("Class 1", x => "None")
            .AddColumn("Class 2", x => "None")
            .AddColumn("Class 3", x => "None");
        return generator.GenerateCsv(teams);
    }

    string ICsvGenerator<StandingsModel>.ExportCsv(StandingsModel standings)
    {
        var generator = new CsvGenerator<StandingRowModel>(",")
            .AddColumn("First name", x => x.Firstname)
            .AddColumn("Last name", x => x.Lastname)
            .AddColumn("Suffix", x => string.Empty)
            .AddColumn("Multicar team name", x => x.TeamName ?? string.Empty)
            .AddColumn("Club name", x => x.ClubName ?? string.Empty)
            .AddColumn("iRacing ID", x => string.Empty)
            .AddColumn("Car number", x => string.Empty)
            .AddColumn("Multicar team background color", x => ConvertToSdkColor(x.TeamColor, hasAlphaValue: true))
            .AddColumn("iRacing car color", x => string.Empty)
            .AddColumn("iRacing car number color", x => string.Empty)
            .AddColumn("iRacing car number color 2", x => string.Empty)
            .AddColumn("iRacing car number color 3", x => string.Empty)
            .AddColumn("iRacing car number font ID", x => string.Empty)
            .AddColumn("iRacing car number style", x => string.Empty)
            .AddColumn("Points before weekend", x => x.TotalPoints.ToString())
            .AddColumn("Points earned", x => "0")
            .AddColumn("Bonus points", x => "0")
            .AddColumn("Points after weekend", x => x.TotalPoints.ToString());

        return generator.GenerateCsv(standings.StandingRows);
    }

    private record GridRowModel(RosterModel Roster, RosterMemberModel Entry);

    static string GetProfileInforOrDefault(RosterMemberModel entry, string key)
    {
        return entry.Profile.TryGetValue(key, out var info) ? info : entry.Member.Profile.TryGetValue(key, out info) ? info : string.Empty;
    }

    string ICsvGenerator<IEnumerable<RosterModel>>.ExportCsv(IEnumerable<RosterModel> data)
    {
        var rows = data.SelectMany(roster => roster.RosterEntries.Select(entry => new GridRowModel(roster, entry))).ToList();
        var generator = new CsvGenerator<GridRowModel>(",")
            .AddColumn("iRacing name", x => $"{x.Entry.Member.Firstname} {x.Entry.Member.Lastname}".Trim())
            .AddColumn("iRacing ID", x => x.Entry.Member.IRacingId ?? string.Empty)
            .AddColumn("Multicar team background color", x => "Transparent")
            .AddColumn("Multicar team text color", x => "Transparent")
            .AddColumn("Multicar team logo url", x => GetProfileInforOrDefault(x.Entry, "Multicar team logo url"))
            .AddColumn("iRacing car color override", x => "Transparent")
            .AddColumn("iRacing car number color override", x => "Transparent")
            .AddColumn("First name override", x => x.Entry.Member.Firstname)
            .AddColumn("Last name override", x => x.Entry.Member.Lastname)
            .AddColumn("Suffix override", x => GetProfileInforOrDefault(x.Entry, "Suffix override"))
            .AddColumn("Initials override", x => GetProfileInforOrDefault(x.Entry, "Initials override"))
            .AddColumn("iRacing team name override", x => x.Entry.Member.TeamName)
            .AddColumn("Multicar team name", x => x.Entry.Member.TeamName)
            .AddColumn("Highlight", x => "None")
            .AddColumn("Club name override", x => "None")
            .AddColumn("Photo URL", x => GetProfileInforOrDefault(x.Entry, "Photo URL"))
            .AddColumn("Number URL", x => GetProfileInforOrDefault(x.Entry, "Number URL"))
            .AddColumn("Car Url", x => GetProfileInforOrDefault(x.Entry, "Car URL"))
            .AddColumn("Class 1", x => x.Roster.Name)
            .AddColumn("Class 2", x => "None")
            .AddColumn("Class 3", x => "None")
            .AddColumn("Birth date", x => GetProfileInforOrDefault(x.Entry, "Birth date"))
            .AddColumn("Home town", x => GetProfileInforOrDefault(x.Entry, "Home town"))
            .AddColumn("Driver header", x => GetProfileInforOrDefault(x.Entry, "Driver header"))
            .AddColumn("Driver information", x => GetProfileInforOrDefault(x.Entry, "Driver information"));
        return generator.GenerateCsv(rows);
    }

    static string[] iRacingClassColors = [
        "ffffda59",
        "ff33ceff",
        "ffff5888",
        "ffae6bff",
        "ff53ff77",
        ];

    string ICsvGenerator<IEnumerable<RosterInfoModel>>.ExportCsv(IEnumerable<RosterInfoModel> data)
    {
        var generator = new CsvGenerator<RosterInfoModel>(",")
            .AddColumn("iRacing class", x => "None")
            .AddColumn("Name", x => x.Name)
            .AddColumn("iRacing color", (x, rowIndex) => $"Class {rowIndex + 1}")
            .AddColumn("Background color override", (x, rowIndex) => rowIndex < iRacingClassColors.Length ? iRacingClassColors[rowIndex] : "Transparent")
            .AddColumn("Text color override", x => "Transparent")
            .AddColumn("Qualify point system", x => "None")
            .AddColumn("Race point system", x => "None")
            .AddColumn("Heat point system", x => "None")
            .AddColumn("Consolation point system", x => "None")
            .AddColumn("Feature point system", x => "None")
            .AddColumn("Class set", x => "Class set 1");

        return generator.GenerateCsv(data);
    }
}

using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Common.Models.Standings;

namespace iRLeagueManager.Web.Data.CsvExporter;

public class SdkGamingCsvGenerator : IMemberListCsvGenerator, ITeamListCsvGenerator, IStandingsCsvGenerator
{
    private static string ConvertToSdkColor(string color)
    {
        if (string.IsNullOrEmpty(color))
        {
            return "Transparent";
        }
        if (color.StartsWith("#"))
        {
            return color.Substring(1);
        }
        return color;
    }

    public string GetName()
    {
        return "SDK Gaming CSV";
    }

    string IMemberListCsvGenerator.ExportCsv(IEnumerable<MemberModel> members)
    {
        var generator = new CsvGenerator<MemberModel>(",")
            .AddColumn("iRacing name", x => $"{x.Firstname} {x.Lastname}".Trim())
            .AddColumn("iRacing ID", x => x.IRacingId ?? string.Empty)
            .AddColumn("Multicar team background color", x => "Transparent")
            .AddColumn("Multicar team text color", x => "Transparent")
            .AddColumn("Multicar team logo url", x => string.Empty)
            .AddColumn("iRacing car color override", x => "Transparent")
            .AddColumn("iRacing car number color override", x => "Transparent")
            .AddColumn("First name override", x => x.Firstname)
            .AddColumn("Last name override", x => x.Lastname)
            .AddColumn("Suffix override", x => string.Empty)
            .AddColumn("Initials override", x => string.Empty)
            .AddColumn("iRacing team name override", x => x.TeamName)
            .AddColumn("Multicar team name", x => x.TeamName)
            .AddColumn("Highlight", x => "None")
            .AddColumn("Club name override", x => "None")
            .AddColumn("Photo URL", x => string.Empty)
            .AddColumn("Number URL", x => string.Empty)
            .AddColumn("Car Url", x => string.Empty)
            .AddColumn("Class 1", x => "None")
            .AddColumn("Class 2", x => "None")
            .AddColumn("Class 3", x => "None")
            .AddColumn("Birth date", x => string.Empty)
            .AddColumn("Home town", x => string.Empty)
            .AddColumn("Driver header", x => string.Empty)
            .AddColumn("Driver information", x => string.Empty);
        return generator.GenerateCsv(members);
    }

    string ITeamListCsvGenerator.ExportCsv(IEnumerable<TeamModel> teams)
    {
        var generator = new CsvGenerator<TeamModel>(",")
            .AddColumn("Name", x => x.Name ?? string.Empty)
            .AddColumn("Background color", x => "Transparent")
            .AddColumn("Text color", x => ConvertToSdkColor(x.TeamColor))
            .AddColumn("Logo URL", x => string.Empty)
            .AddColumn("Class 1", x => "None")
            .AddColumn("Class 2", x => "None")
            .AddColumn("Class 3", x => "None");
        return generator.GenerateCsv(teams);
    }

    string IStandingsCsvGenerator.ExportCsv(StandingsModel standings)
    {
        var generator = new CsvGenerator<StandingRowModel>(",")
            .AddColumn("First name", x => x.Firstname)
            .AddColumn("Last name", x => x.Lastname)
            .AddColumn("Suffix", x => string.Empty)
            .AddColumn("Multicar team name", x => x.TeamName ?? string.Empty)
            .AddColumn("Club name", x => x.ClubName ?? string.Empty)
            .AddColumn("iRacing ID", x => string.Empty)
            .AddColumn("Car number", x => string.Empty)
            .AddColumn("Multicar team background color", x => ConvertToSdkColor(x.TeamColor))
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
}

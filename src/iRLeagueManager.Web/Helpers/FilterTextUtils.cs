using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueManager.Web.Helpers;

internal static class FilterTextUtils
{
    public static string GetConditionText(
        FilterConditionModel condition,
        IEnumerable<MemberInfoModel>? members = default,
        IEnumerable<TeamInfoModel>? teams = default)
    {
        members ??= Array.Empty<MemberInfoModel>();
        teams ??= Array.Empty<TeamInfoModel>();
        return condition.FilterType switch
        {
            FilterType.Member => string.Join(", ", condition.FilterValues.Select(x => GetMemberName(x, members))),
            FilterType.Team => string.Join(", ", condition.FilterValues.Select(x => GetTeamName(x, teams))),
            _ => $"{condition.Comparator}: {string.Join(", ", condition.FilterValues)}",
        };
    }

    private static string GetMemberName(string id, IEnumerable<MemberInfoModel> members)
    {
        if (long.TryParse(id, out long idValue) == false)
        {
            return id;
        }
        var member = members.FirstOrDefault(x => x.MemberId == idValue);
        if (member is null)
        {
            return id;
        }
        return $"{member.FirstName} {member.LastName}";
    }

    private static string GetTeamName(string id, IEnumerable<TeamInfoModel> teams)
    {
        if (long.TryParse(id, out long idValue) == false)
        {
            return id;
        }
        var team = teams.FirstOrDefault(x => x.TeamId == idValue);
        if (team is null)
        {
            return id;
        }
        return team.Name;
    }
}

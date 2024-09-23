using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;

namespace iRLeagueManager.Web.Helpers;

internal static class FilterTextUtils
{
    public static string GetConditionText(
        this FilterConditionModel condition,
        IEnumerable<MemberModel>? members = default,
        IEnumerable<TeamModel>? teams = default)
    {
        members ??= [];
        teams ??= [];
        return condition.FilterType switch
        {
            FilterType.Member => string.Join(", ", condition.FilterValues.Select(x => GetMemberName(x, members))),
            FilterType.Team => string.Join(", ", condition.FilterValues.Select(x => GetTeamName(x, teams))),
            _ => $"{condition.Comparator.GetText()} {string.Join(", ", condition.FilterValues.Select(x => GetValueText(GetColumnPropertyType(condition.ColumnPropertyName), x)))}",
        };
    }

    private static Type? GetColumnPropertyType(string name)
    {
        var property = typeof(ResultRowModel).GetProperty(name);
        return property?.PropertyType;
    }

    private static string GetValueText(Type? valueType, string value)
    {
        if (valueType == typeof(RaceStatus))
        {
            return Enum.TryParse(value, out RaceStatus status) ? status.ToString() : value;
        }
        return value;
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

    private static string GetTeamName(string id, IEnumerable<TeamModel> teams)
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

    public static string GetText(this ComparatorType comparatorType)
    {
        return comparatorType switch
        {
            ComparatorType.IsSmaller => "<",
            ComparatorType.IsSmallerOrEqual => "<=",
            ComparatorType.IsEqual => "==",
            ComparatorType.IsBiggerOrEqual => ">=",
            ComparatorType.IsBigger => ">",
            ComparatorType.NotEqual => "!=",
            ComparatorType.InList => "in",
            ComparatorType.ForEach => "each",
            ComparatorType.Min => "min",
            ComparatorType.Max => "max",
            _ => "",
        };
    }
}

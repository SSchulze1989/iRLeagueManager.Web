using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace iRLeagueApiCore.Common;

public static class LeagueRoles
{
    /// <summary>
    /// Member with restricted read access to public information:
    /// schedules, sessions, results - but not incident reviews
    /// </summary>
    public const string Member = "Member";
    public static LeagueRoleValue MemberValue { get; } = new(Member);
    /// <summary>
    /// Member with read access to reviews and allowed to create and edit own reviews
    /// No write access to schedules, sessions or results
    /// </summary>
    public const string Steward = "Steward";
    public static LeagueRoleValue StewardValue { get; } = new(Steward, new[] { MemberValue });
    /// <summary>
    /// Organizer of the league
    /// Write privileges but not allowed to delete seasons or assign roles
    /// </summary>
    public const string Organizer = "Organizer";
    public static LeagueRoleValue OrganizerValue { get; } = new(Organizer, new[] { MemberValue });
    /// <summary>
    /// Administrator of the league with full privileges
    /// </summary>
    public const string Admin = "Admin";
    public static LeagueRoleValue AdminValue { get; } = new(Admin, new[] { OrganizerValue, StewardValue });
    /// <summary>
    /// Owner of the league
    /// Full privilige including right to delete
    /// </summary>
    public const string Owner = "Owner";
    public static LeagueRoleValue OwnerValue { get; } = new(Owner, new[] { AdminValue });

    /// <summary>
    /// Array of all available league roles
    /// </summary>
    public static LeagueRoleValue[] RolesAvailable { get; } = new[]
    {
        OwnerValue,
        AdminValue,
        OrganizerValue,
        StewardValue,
        MemberValue
    };

    /// <summary>
    /// Delimiter used to separate between league name and role name
    /// </summary>
    public const char RoleDelimiter = ':';

    /// <summary>
    /// Get the full league role name for a provided league name and role name
    /// </summary>
    /// <returns><see langword="null"/> if <paramref name="roleName"/> is not valid</returns>
    public static string? GetLeagueRoleName(string leagueName, string roleName)
    {
        if (IsValidRole(roleName) == false)
        {
            return null;
        }

        return $"{leagueName.ToLower()}{RoleDelimiter}{CapitalizeRoleName(roleName)}";
    }

    public static string? GetRoleName(string leagueRoleName)
    {
        return leagueRoleName.Split(RoleDelimiter).ElementAtOrDefault(1);
    }

    /// <summary>
    /// Check wether a full league role name is a valid role name for the provided league
    /// </summary>
    /// <returns></returns>
    public static bool IsLeagueRoleName(string leagueName, string roleName)
    {
        var pattern = $"({leagueName})({RoleDelimiter})({string.Join<LeagueRoleValue>('|', RolesAvailable)})";
        return Regex.IsMatch(roleName, pattern, RegexOptions.IgnoreCase);
    }

    public static bool IsValidRole(string roleName)
    {
        return RolesAvailable.Any(x => x.Equals(roleName));
    }

    private static string CapitalizeRoleName(string roleName)
    {
        return char.ToUpper(roleName[0]) + roleName[1..].ToLower();
    }

    /// <summary>
    /// Get the role value for a valid roleName
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If <paramref name="roleName"/> is not a valid roleName</exception>
    public static LeagueRoleValue GetRoleValue(string roleName)
    {
        return RolesAvailable.FirstOrDefault(x => x.Equals(roleName))
            ?? throw new ArgumentException($"{roleName} is not a valid role", nameof(roleName));
    }

    public static bool CheckRole(LeagueRoleValue role, IEnumerable<LeagueRoleValue> UserRoles)
    {
        return UserRoles.Any(x => x.HasRole(role));
    }

    public static IEnumerable<LeagueRoleValue> ImplicitRoleOf(LeagueRoleValue role)
    {
        return RolesAvailable.
            Where(x => x.GetImplicitRoles().Contains(role));
    }

    private class RoleComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            return x?.Equals(y, System.StringComparison.OrdinalIgnoreCase) == true;
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }
}

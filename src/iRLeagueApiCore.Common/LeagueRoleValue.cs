namespace iRLeagueApiCore.Common;

public class LeagueRoleValue : IEquatable<LeagueRoleValue>, IEquatable<string>
{
    private readonly string value;
    private readonly LeagueRoleValue[] implicitRoles;

    internal LeagueRoleValue(string value)
    {
        this.value = value;
        implicitRoles = Array.Empty<LeagueRoleValue>();
    }

    internal LeagueRoleValue(string value, IEnumerable<LeagueRoleValue> implicitRoles)
    {
        this.value = value;
        this.implicitRoles = implicitRoles
            .SelectMany(x => new[] { x }.Concat(x.GetImplicitRoles()))
            .ToArray();
    }

    public static implicit operator string(LeagueRoleValue role) => role.value;

    public bool Equals(LeagueRoleValue? other)
    {
        return value.Equals(other?.value, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(string? other)
    {
        return value.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return value;
    }

    public IEnumerable<LeagueRoleValue> GetImplicitRoles()
    {
        return implicitRoles.ToArray();
    }

    /// <summary>
    /// Check wether this role is contained by this RoleValue or any of its implicit roles
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public bool HasRole(string role)
    {
        return value.Equals(role) || implicitRoles.Any(x => x.HasRole(role));
    }

    /// <summary>
    /// Check wether this role is contained by this RoleValue or any of its implicit roles
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public bool HasRole(LeagueRoleValue role)
    {
        return HasRole(role.ToString()) || implicitRoles.Any(x => x.HasRole(role));
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj?.ToString());
    }

    public override int GetHashCode()
    {
        return value.ToLower().GetHashCode();
    }

    public static bool operator ==(LeagueRoleValue left, LeagueRoleValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LeagueRoleValue left, LeagueRoleValue right)
    {
        return !(left == right);
    }
}

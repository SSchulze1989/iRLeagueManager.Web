namespace iRLeagueApiCore.Server.Filters;

/// <summary>
/// Sets requirement for having at least one of the listed league roles to access the resource
/// Only works in combination with <see cref="LeagueAuthorizeAttribute"/>
/// <para>If no league role is specified, the user is checked to be in at least one of any available league role</para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class RequireLeagueRoleAttribute : Attribute
{
    /// <summary>
    /// Private field to prevent modifiying from outside
    /// </summary>
    private readonly LeagueRoleValue[] _roles;
    /// <summary>
    /// Roles that are required to access the decorated resource
    /// </summary>
    public LeagueRoleValue[] Roles => _roles.ToArray();
    /// <summary>
    /// </summary>
    /// <param name="roles">List of roles. Requires user to be in at least one of the provided roles</param>
    public RequireLeagueRoleAttribute(params string[] roles)
    {
        var roleValues = roles.Select(x => LeagueRoles.GetRoleValue(x));
        var allRoles = IncludeImplicitRoles(roleValues);
        _roles = allRoles.ToArray();
    }

    /// <summary>
    /// Includes the (reverse) implicit role requirements
    /// For example "Admin" is always implicit in role "Owner", that means "Owner" will satisfy all conditions where "Admin" is required
    /// Therefore "Owner" is automatically added to the required roles list
    /// </summary>
    /// <param name="roles"></param>
    /// <returns></returns>
    private static IEnumerable<LeagueRoleValue> IncludeImplicitRoles(IEnumerable<LeagueRoleValue> roles)
    {
        return roles.Concat(roles.SelectMany(x => LeagueRoles.ImplicitRoleOf(x))).Distinct();
    }
}

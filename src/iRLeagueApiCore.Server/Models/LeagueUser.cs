using iRLeagueApiCore.Server.Extensions;
using iRLeagueApiCore.Services.ResultService.Extensions;
using System.Security.Claims;

namespace iRLeagueApiCore.Server.Models;

public sealed class LeagueUser
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyCollection<LeagueRoleValue> Roles { get; private set; }

    public LeagueUser(string leagueName, ClaimsPrincipal principal)
    {
        Id = principal.GetUserId() ?? string.Empty;
        Name = principal.Identity?.Name ?? string.Empty;
        if (string.IsNullOrEmpty(leagueName))
        {
            Roles = principal.FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .Where(role => role == "Admin")
                .Select(LeagueRoles.GetRoleValue)
                .ToList();
        }
        else
        {
            Roles = principal.FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .Where(role => LeagueRoles.IsLeagueRoleName(leagueName, role) || role == "Admin")
                .Select(role => role == "Admin" ? role : LeagueRoles.GetRoleName(role))
                .NotNull()
                .Select(LeagueRoles.GetRoleValue)
                .ToList();
        }
    }

    /// <summary>
    /// Returns true when the user is in at least one of the provided roles
    /// </summary>
    /// <param name="roles"></param>
    /// <returns></returns>
    public bool IsInRole(params string[] roles)
    {
        return Roles.Any(userRole => roles.Any(requiredRole => userRole.HasRole(requiredRole)));
    }

    public static LeagueUser Empty => new LeagueUser("", new ClaimsPrincipal());
}

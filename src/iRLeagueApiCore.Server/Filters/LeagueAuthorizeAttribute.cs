using iRLeagueApiCore.Common.Responses;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Diagnostics;
using System.Security.Principal;

namespace iRLeagueApiCore.Server.Filters;

/// <summary>
/// Authorization filter to manage access to league resources bases on user roles specific to each league
/// <para>The pattern for league roles is {leagueName}:{roleName} so for example an admin for testleague must be in the role: testleague:Admin</para>
/// <para><b>The decorated class or method must have {leagueName} as a route parameter.</b></para> 
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class LeagueAuthorizeAttribute : ActionFilterAttribute
{
    private readonly ILogger<LeagueAuthorizeAttribute> _logger;
    private readonly LeagueDbContext dbContext;
    private readonly IDiagnosticContext diagnosticContext;

    public LeagueAuthorizeAttribute(ILogger<LeagueAuthorizeAttribute> logger, IDiagnosticContext diagnosticContext, LeagueDbContext dbContext)
    {
        _logger = logger;
        this.diagnosticContext = diagnosticContext;
        this.dbContext = dbContext;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // infer league name from context
        var leagueName = await GetLeagueNameFromContext(context);
        if (string.IsNullOrEmpty(leagueName))
        {
            _logger.LogError("Missing [leagueName] or [leagueId] parameter in action route");
            throw new InvalidOperationException("Missing [leagueName] or [leagueId] in action route");
        }

        // get user from httpcontext
        var user = context.HttpContext.User;
        var userName = (user.Identity != null && user.Identity.IsAuthenticated) ? user.Identity.Name : "Anonymous";
        var leagueUser = new LeagueUser(leagueName, user);

        // _logger.LogDebug("Authorizing request for {UserName} on {leagueName}", userName, leagueName);

        // check if specific league role required
        var requireLeagueRoleAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<RequireLeagueRoleAttribute>();
        var allowAnonymousAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .FirstOrDefault();
        if (allowAnonymousAttribute != null || requireLeagueRoleAttribute.Any() == false)
        {
            // Allow public access
            await AccessGranted(context, next);
            return;
        }

        if (user == null || user.Identity == null || user.Identity.IsAuthenticated == false)
        {
            _logger.LogInformation("Permission denied for Anonymous user on {LeagueName}. League is not public", leagueName);
            context.Result = new UnauthorizedObjectResult(new UnauthorizedResponse()
            {
                Status = "Not logged in",
                Errors = new object[] { "Not logged in to user account" },
            });
            return;
        }

        var requiredRoles = requireLeagueRoleAttribute
            .SelectMany(x => x.Roles)
            .ToList();
        if (requiredRoles.Count > 0)
        {
            var hasRole = requiredRoles
                .Any(x => HasLeagueRole(user, leagueName, x));

            if (hasRole == false)
            {
                _logger.LogInformation("Permission denied for {User} on {LeagueName}. User is not in any required role {Roles}",
                    user.Identity.Name, leagueName, requiredRoles);
                AccessDenied(context, next);
                return;
            }
        }
        else if (HasAnyLeagueRole(user, leagueName) == false)
        {
            _logger.LogInformation("Permission denied for {User} on {LeagueName}. User is not in any league role", user.Identity.Name, leagueName);
            AccessDenied(context, next);
            return;
        }

        await AccessGranted(context, next);
    }

    private static bool HasAnyLeagueRole(IPrincipal user, string leagueName)
    {
        foreach (var roleName in LeagueRoles.RolesAvailable)
        {
            var leagueRole = LeagueRoles.GetLeagueRoleName(leagueName, roleName);
            if (leagueRole != null && user.IsInRole(leagueRole))
            {
                return true;
            }
        }
        return user.IsInRole(UserRoles.Admin);
    }

    private static bool HasLeagueRole(IPrincipal user, string leagueName, string roleName)
    {
        var leagueRole = LeagueRoles.GetLeagueRoleName(leagueName, roleName);
        if (leagueRole == null)
        {
            return false;
        }
        return user.IsInRole(leagueRole) || user.IsInRole(UserRoles.Admin);
    }

    private async Task<string?> GetLeagueNameFromContext(ActionExecutingContext context)
    {
        if (context.RouteData.Values.TryGetValue("leagueName", out var leagueNameObject))
        {
            return (string)leagueNameObject!;
        }
        if (context.RouteData.Values.TryGetValue("leagueId", out var leagueIdObject) == false)
        {
            return null;
        }
        if (long.TryParse((string)leagueIdObject!, out long leagueId) == false)
        {
            return null;
        }
        return (await dbContext.Leagues
            .Where(x => x.Id == leagueId)
            .FirstOrDefaultAsync())
            ?.Name;
    }

    private void AccessDenied(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.Result = new ForbidResult();
    }

    private async Task AccessGranted(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await base.OnActionExecutionAsync(context, next);
    }
}

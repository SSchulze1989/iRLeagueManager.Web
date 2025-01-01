using Microsoft.AspNetCore.Authorization;

namespace iRLeagueManager.Web.Shared;

public class CustomAuthorizationHandler : AuthorizationHandler<IAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (context.Resource != null)
        {
            // Bypass authorization
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

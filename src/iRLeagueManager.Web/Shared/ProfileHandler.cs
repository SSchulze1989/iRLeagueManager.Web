using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using iRLeagueManager.Web.Extensions;

namespace iRLeagueManager.Web.Shared;

public class ProfileHandler : AuthorizationHandler<ProfileOwnerRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileOwnerRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var profileId = context.Resource?.ToString() ?? string.Empty;

            if (IsOwner(context.User, profileId))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }

    private static bool IsOwner(ClaimsPrincipal user, string profileId)
    {
        var userProfileId = user.GetUserId();

        return userProfileId == profileId;
    }
}


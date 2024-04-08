using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Validation.Leagues;

public sealed class PostLeagueRequestValidator : AbstractValidator<PostLeagueRequest>
{
    private const int maxLeagues = 3;
    private UserManager<ApplicationUser> userManager;

    public PostLeagueRequestValidator(PostLeagueModelValidator modelValidator, UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;

        RuleFor(x => x.Model)
            .SetValidator(modelValidator);
        RuleFor(x => x.User)
            .MustAsync(HaveLessThanMaximumAllowedLeagues)
            .WithMessage("User is already the owner for the maximum allowed number of leagues");
    }

    private async Task<bool> HaveLessThanMaximumAllowedLeagues(LeagueUser user, CancellationToken cancellationToken)
    {
        var applicationUser = await userManager.FindByIdAsync(user.Id);
        if (applicationUser is null)
        {
            // User does not exist but that will be handled at a different point
            // For now no user means -> validation ok
            return true;
        }
        var userLeagueCount = (await userManager
            .GetRolesAsync(applicationUser))
            .Where(x => x.Contains(LeagueRoles.Owner))
            .Count();
        return userLeagueCount < maxLeagues;
    }
}

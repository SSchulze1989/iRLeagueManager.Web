using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Handlers.Admin;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Validation;

public sealed class GiveRoleRequestValidator : AbstractValidator<GiveRoleRequest>
{
    private readonly LeagueDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public GiveRoleRequestValidator(LeagueDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;

        // league name must exist
        RuleFor(x => x.LeagueName)
            .MustAsync(LeagueNameExists)
            .WithMessage(x => "League does not exist");
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage(x => "UserName is required")
            .MustAsync(UserExists)
            .WithMessage(x => "User not found")
            .MustAsync(UserNotInRole)
            .WithMessage(x => "User is already in role");
        RuleFor(x => x.RoleName)
            .Must(RoleIsValid)
            .WithMessage(x => $"Invalid role. Use: either '{string.Join("', '", LeagueRoles.RolesAvailable.SkipLast(1))}' or '{LeagueRoles.RolesAvailable.Last()}'");
    }

    private async Task<bool> LeagueNameExists(string leagueName, CancellationToken cancellationToken)
    {
        return await _dbContext.Leagues
            .AnyAsync(x => leagueName == x.Name, cancellationToken);
    }

    private async Task<bool> UserExists(GiveRoleRequest request, string usernName, CancellationToken cancellationToken)
    {
        var user = await _userManager
            .FindByNameAsync(usernName);
        request.User = user;
        return user != null;
    }

    private bool RoleIsValid(string roleName)
    {
        return LeagueRoles.RolesAvailable
            .Any(x => roleName == x);
    }

    private async Task<bool> UserNotInRole(GiveRoleRequest request, string userName, CancellationToken cancellationToken)
    {
        var leagueRoleName = LeagueRoles.GetLeagueRoleName(request.LeagueName, request.RoleName);
        bool notInRole = await _userManager.IsInRoleAsync(request.User, leagueRoleName) == false;
        return notInRole;
    }
}

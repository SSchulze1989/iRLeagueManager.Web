using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Admin;

public sealed class GiveRoleRequest : UserRoleModel, IRequest
{
    public string LeagueName { get; }
    public ApplicationUser User { get; set; } = default!;

    public GiveRoleRequest(string leagueName, UserRoleModel UserRole)
    {
        LeagueName = leagueName;
        UserName = UserRole.UserName;
        RoleName = UserRole.RoleName;
    }
}

public sealed class GiveRoleHandler : IRequestHandler<GiveRoleRequest>
{
    private readonly ILogger<GiveRoleHandler> _logger;
    private readonly IEnumerable<IValidator<GiveRoleRequest>> _validators;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public GiveRoleHandler(ILogger<GiveRoleHandler> logger, IEnumerable<IValidator<GiveRoleRequest>> validators,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _validators = validators;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Unit> Handle(GiveRoleRequest request, CancellationToken cancellationToken = default)
    {
        await _validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var leagueName = request.LeagueName;
        var roleName = request.RoleName;
        var leagueRoleName = LeagueRoles.GetLeagueRoleName(leagueName, roleName);
        var user = request.User;

        if (leagueRoleName == null)
        {
            return Unit.Value;
        }

        // check if role needs to be created
        if (await _roleManager.RoleExistsAsync(leagueRoleName) == false)
        {
            _logger.LogInformation("Creating league role {LeagueRole} for {LeagueName} => {Role}", roleName,
                leagueName, leagueRoleName);
            await CreateLeagueRoleAsync(leagueRoleName);
            _logger.LogInformation("League role {LeagueRole} for {LeagueName} created => {Role}", roleName,
                leagueName, leagueRoleName);
        }
        await AddUserToRole(user, leagueRoleName);
        _logger.LogInformation("Added league role {LeagueRole} to user {RoleUser} for {LeagueName}",
            roleName, user.UserName, leagueName);
        return Unit.Value;
    }

    private async Task CreateLeagueRoleAsync(string leagueRoleName)
    {
        var roleResult = await _roleManager.CreateAsync(new IdentityRole(leagueRoleName));

        if (roleResult.Succeeded == false)
        {
            _logger.LogError("Failed to create role {Role} due to errors: {Errors}", leagueRoleName,
                roleResult.Errors.Select(x => $"{x.Code}: {x.Description}"));
            throw new InvalidOperationException($"Failed to create role {leagueRoleName}");
        }
    }

    private async Task AddUserToRole(ApplicationUser user, string roleName)
    {
        var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (addToRoleResult.Succeeded == false)
        {
            _logger.LogError("Failed to add user {RoleUser} to role {Role} due to errors: {Errors}",
                user.UserName, roleName,
                addToRoleResult.Errors.Select(x => $"{x.Code}: {x.Description}"));
            throw new InvalidOperationException($"Failed to add {user.UserName} to role {roleName}");
        }
    }
}

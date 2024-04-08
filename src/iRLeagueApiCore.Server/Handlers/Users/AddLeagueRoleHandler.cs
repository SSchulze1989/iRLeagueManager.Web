using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record AddLeagueRoleRequest(string LeagueName, string UserId, string RoleName) : IRequest<LeagueUserModel>;

public sealed class AddLeagueRoleHandler : UsersHandlerBase<AddLeagueRoleHandler, AddLeagueRoleRequest>,
    IRequestHandler<AddLeagueRoleRequest, LeagueUserModel>
{
    private readonly RoleManager<IdentityRole> roleManager;

    public AddLeagueRoleHandler(ILogger<AddLeagueRoleHandler> logger, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, IEnumerable<IValidator<AddLeagueRoleRequest>> validators) : base(logger, userManager, validators)
    {
        this.roleManager = roleManager;
    }

    public async Task<LeagueUserModel> Handle(AddLeagueRoleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var user = await GetUserAsync(request.UserId)
            ?? throw new ResourceNotFoundException();
        await AddLeagueRoleToUser(user, request.LeagueName, request.RoleName);
        var getUser = await MapToLeagueUserModelAsync(user, request.LeagueName, new());
        return getUser;
    }

    private async Task AddLeagueRoleToUser(ApplicationUser user, string leagueName, string roleName)
    {
        var leagueRoleName = LeagueRoles.GetLeagueRoleName(leagueName, roleName);
        if (await roleManager.RoleExistsAsync(leagueRoleName) == false)
        {
            await roleManager.CreateAsync(new() { Name = leagueRoleName });
        }
        await userManager.AddToRoleAsync(user, leagueRoleName);
    }
}

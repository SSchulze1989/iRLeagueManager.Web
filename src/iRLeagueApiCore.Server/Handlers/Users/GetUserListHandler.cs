using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record GetUserListRequest(string LeagueName) : IRequest<IEnumerable<LeagueUserModel>>;

public sealed class GetUserListHandler : UsersHandlerBase<GetUserListHandler, GetUserListRequest>,
    IRequestHandler<GetUserListRequest, IEnumerable<LeagueUserModel>>
{
    public GetUserListHandler(ILogger<GetUserListHandler> logger, UserManager<ApplicationUser> userManager,
        IEnumerable<IValidator<GetUserListRequest>> validators) : base(logger, userManager, validators)
    {
    }

    public async Task<IEnumerable<LeagueUserModel>> Handle(GetUserListRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var leagueUsers = await GetUserEntitiesWithLeagueRole(request.LeagueName, cancellationToken);
        var getUsers = await MapToLeagueUserListAsync(leagueUsers, request.LeagueName, cancellationToken);
        return getUsers;
    }

    private async Task<IEnumerable<ApplicationUser>> GetUserEntitiesWithLeagueRole(string leagueName, CancellationToken cancellationToken)
    {
        var leagueRoles = LeagueRoles.RolesAvailable
            .Select(x => LeagueRoles.GetLeagueRoleName(leagueName, x));
        var usersWithRole = new List<ApplicationUser>();
        foreach (var leagueRole in leagueRoles)
        {
            usersWithRole.AddRange(await userManager.GetUsersInRoleAsync(leagueRole));
        }
        return usersWithRole.DistinctBy(x => x.Id);
    }

    private async Task<IEnumerable<LeagueUserModel>> MapToLeagueUserListAsync(IEnumerable<ApplicationUser> users, string leagueName, CancellationToken cancellationToken)
    {
        var userModels = new List<LeagueUserModel>();
        foreach (var user in users)
        {
            var model = await MapToLeagueUserModelAsync(user, leagueName, new());
            userModels.Add(model);
        }
        return userModels;
    }
}

using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record GetLeagueUserRequest(string LeagueName, string UserId) : IRequest<LeagueUserModel>;
public sealed class GetLeagueUserHandler : UsersHandlerBase<GetLeagueUserHandler, GetLeagueUserRequest>, IRequestHandler<GetLeagueUserRequest, LeagueUserModel>
{
    public GetLeagueUserHandler(ILogger<GetLeagueUserHandler> logger, UserDbContext userDbContext, UserManager<ApplicationUser> userManager,
        IEnumerable<IValidator<GetLeagueUserRequest>> validators) : base(logger, userManager, validators)
    {
    }

    public async Task<LeagueUserModel> Handle(GetLeagueUserRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var user = await GetUserAsync(request.UserId)
            ?? throw new ResourceNotFoundException();
        var getUser = await MapToLeagueUserModelAsync(user, request.LeagueName, new());
        if (getUser.LeagueRoles.Any() == false)
        {
            // return not found if user does not have any league role
            // meaning this is not a LeagueUser (yet)
            throw new ResourceNotFoundException();
        }
        return getUser;
    }
}

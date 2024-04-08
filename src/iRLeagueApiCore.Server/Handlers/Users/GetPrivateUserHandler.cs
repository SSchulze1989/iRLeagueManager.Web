using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record GetPrivateUserRequest(string UserId) : IRequest<PrivateUserModel>;

public sealed class GetPrivateUserHandler : UsersHandlerBase<GetPrivateUserHandler, GetPrivateUserRequest>, IRequestHandler<GetPrivateUserRequest, PrivateUserModel>
{
    public GetPrivateUserHandler(ILogger<GetPrivateUserHandler> logger, UserDbContext userDbContext, UserManager<ApplicationUser> userManager,
        IEnumerable<IValidator<GetPrivateUserRequest>> validators) : base(logger, userManager, validators)
    {
    }

    public async Task<PrivateUserModel> Handle(GetPrivateUserRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var user = await GetUserAsync(request.UserId)
            ?? throw new ResourceNotFoundException();
        var getUser = MapToPrivateUserModel(user, new());
        return getUser;
    }
}

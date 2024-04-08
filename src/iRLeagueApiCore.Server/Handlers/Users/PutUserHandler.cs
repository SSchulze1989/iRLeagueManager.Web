using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record PutUserRequest(string UserId, PutUserModel Model) : IRequest<PrivateUserModel>;

public sealed class PutUserHandler : UsersHandlerBase<PutUserHandler, PutUserRequest>, IRequestHandler<PutUserRequest, PrivateUserModel>
{
    private readonly UserDbContext userDbContext;

    public PutUserHandler(ILogger<PutUserHandler> logger, UserDbContext userDbContext, UserManager<ApplicationUser> userManager,
        IEnumerable<IValidator<PutUserRequest>> validators) : base(logger, userManager, validators)
    {
        this.userDbContext = userDbContext;
    }

    public async Task<PrivateUserModel> Handle(PutUserRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var user = await GetUserEntityAsync(request.UserId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        user = MapToUserEntity(request.Model, user);
        await userDbContext.SaveChangesAsync(cancellationToken);
        var getUser = MapToPrivateUserModel(user, new PrivateUserModel());
        return getUser;
    }

    private ApplicationUser MapToUserEntity(PutUserModel model, ApplicationUser user)
    {
        user.FullName = GetUserFullName(model.Firstname, model.Lastname);
        user.Email = model.Email;
        user.HideFullName = model.HideFirstnameLastname;
        return user;
    }

    private async Task<ApplicationUser?> GetUserEntityAsync(string? userId, CancellationToken cancellationToken)
    {
        return await userDbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }
}

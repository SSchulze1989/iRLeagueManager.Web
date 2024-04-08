using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Users;

public record SearchUsersByNameRequest(string[] SearchKeys) : IRequest<IEnumerable<UserModel>>;

public sealed class SearchUsersByNameHandler : UsersHandlerBase<SearchUsersByNameHandler, SearchUsersByNameRequest>,
    IRequestHandler<SearchUsersByNameRequest, IEnumerable<UserModel>>
{
    private readonly UserDbContext userDbContext;

    public SearchUsersByNameHandler(ILogger<SearchUsersByNameHandler> logger, UserDbContext userDbContext, UserManager<ApplicationUser> userManager,
        IEnumerable<IValidator<SearchUsersByNameRequest>> validators) : base(logger, userManager, validators)
    {
        this.userDbContext = userDbContext;
    }

    public async Task<IEnumerable<UserModel>> Handle(SearchUsersByNameRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var users = await SearchUsers(request.SearchKeys);
        var getUsers = users.Select(x => MapToUserModel(x, new()));
        return getUsers;
    }

    private async Task<IEnumerable<ApplicationUser>> SearchUsers(params string[] searchKeys)
    {
        var regexString = string.Join('|', searchKeys.Select(x => x.ToLower()));
        return await userDbContext.Users
            .FromSqlInterpolated($"SELECT * FROM AspNetUsers WHERE LOWER(UserName) REGEXP {regexString} OR LOWER(FullName) REGEXP {regexString}")
            .ToListAsync();
    }
}

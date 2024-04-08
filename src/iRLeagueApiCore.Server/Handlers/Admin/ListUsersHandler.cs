using iRLeagueApiCore.Common.Models.Users;
using iRLeagueApiCore.Server.Authentication;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Admin;

public record ListUsersRequest(string LeagueName) : IRequest<IEnumerable<AdminUserModel>>;

public sealed class ListUsersHandler : IRequestHandler<ListUsersRequest, IEnumerable<AdminUserModel>>
{
    private readonly ILogger<ListUsersHandler> _logger;
    private readonly IEnumerable<IValidator<ListUsersRequest>> _validators;
    private readonly UserManager<ApplicationUser> _userManager;

    public ListUsersHandler(ILogger<ListUsersHandler> logger, IEnumerable<IValidator<ListUsersRequest>> validators,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _validators = validators;
        _userManager = userManager;
    }

    public async Task<IEnumerable<AdminUserModel>> Handle(ListUsersRequest request, CancellationToken cancellationToken = default)
    {
        await _validators.ValidateAllAndThrowAsync(request, cancellationToken);
        // Get users that have a league role
        var users = await GetUsersWithLeagueRoleAsync(request.LeagueName);
        var getUsers = users.Select(x => MapToAdminUserModel(x.Key, x));
        return getUsers;
    }

    private async Task<IEnumerable<IGrouping<ApplicationUser, string>>> GetUsersWithLeagueRoleAsync(string leagueName)
    {
        var users = new List<(ApplicationUser user, string role)>();
        foreach (var role in LeagueRoles.RolesAvailable)
        {
            var leagueRoleName = LeagueRoles.GetLeagueRoleName(leagueName, role);
            var inRole = await _userManager.GetUsersInRoleAsync(leagueRoleName);
            if (inRole != null)
            {
                users.AddRange(inRole.Select(user => (user, (string)role)));
            }
        }
        return users.GroupBy(x => x.user, x => x.role);
    }

    private static AdminUserModel MapToAdminUserModel(ApplicationUser user, IEnumerable<string> roles)
    {
        var parts = user.FullName?.Split(';') ?? Array.Empty<string>();
        return new AdminUserModel()
        {
            UserName = user.UserName,
            Firstname = parts.ElementAtOrDefault(0) ?? string.Empty,
            Lastname = parts.ElementAtOrDefault(1) ?? string.Empty,
            Email = user.Email,
            Roles = roles,
        };
    }
}

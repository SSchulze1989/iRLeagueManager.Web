using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Authentication;
using iRLeagueApiCore.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace iRLeagueApiCore.Server.Handlers.Leagues;

public record PostLeagueRequest(LeagueUser User, PostLeagueModel Model) : IRequest<LeagueModel>;

public sealed class PostLeagueHandler : LeagueHandlerBase<PostLeagueHandler, PostLeagueRequest>, IRequestHandler<PostLeagueRequest, LeagueModel>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public PostLeagueHandler(ILogger<PostLeagueHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<PostLeagueRequest>> validators, UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager)
        : base(logger, dbContext, validators)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public async Task<LeagueModel> Handle(PostLeagueRequest request, CancellationToken cancellationToken = default)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        _logger.LogInformation("Create league {LeagueName}", request.Model.Name);
        var leagueEntity = MapToLeagueEntity(request.User, request.Model, new LeagueEntity());
        dbContext.Leagues.Add(leagueEntity);
        if (await AssignOwnerRole(request.User, leagueEntity.Name) == false)
        {
            throw new InvalidOperationException("Failed to add owner rule to user. Aborting league creation");
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("League {LeagueName} successfully created", request.Model.Name);
        var getLeague = await MapToGetLeagueModelAsync(leagueEntity.Id, true, cancellationToken) 
            ?? throw new ResourceNotFoundException("Created resource not found!");
        return getLeague;
    }

    /// <summary>
    /// Assign the owner role to the new created league
    /// </summary>
    /// <param name="user"></param>
    /// <param name="leagueName"></param>
    /// <returns><see langword="true"/> when role was assigned successfully; <see langword="false"/> if role was not assigned</returns>
    public async Task<bool> AssignOwnerRole(LeagueUser user, string leagueName)
    {
        // create owner role if not exists
        var ownerRole = LeagueRoles.GetLeagueRoleName(leagueName, LeagueRoles.Owner);
        if (await roleManager.RoleExistsAsync(ownerRole) == false)
        {
            var createdRole = await roleManager.CreateAsync(new(ownerRole));
            if (createdRole is null)
            {
                return false;
            }
        }
        var applicationUser = await userManager.FindByIdAsync(user.Id);
        if (applicationUser is null)
        {
            return false;
        }
        if (await userManager.IsInRoleAsync(applicationUser, ownerRole))
        {
            return true;
        }
        var result = await userManager.AddToRoleAsync(applicationUser, ownerRole);
        return result.Succeeded;
    }
}

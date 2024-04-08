using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Teams;

public record PostTeamRequest(LeagueUser User, PostTeamModel Model) : IRequest<TeamModel>;

public class PostTeamHandler : TeamsHandlerBase<PostTeamHandler, PostTeamRequest>, 
    IRequestHandler<PostTeamRequest, TeamModel>
{
    public PostTeamHandler(ILogger<PostTeamHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PostTeamRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<TeamModel> Handle(PostTeamRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postTeam = await CreateTeamEntity(request.User, cancellationToken);
        postTeam = await MapToTeamEntityAsync(request.User, request.Model, postTeam, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getTeam = await MapToTeamModel(postTeam.TeamId, cancellationToken)
            ?? throw new InvalidOperationException("Failed to fetch created team resource");
        return getTeam;
    }
}

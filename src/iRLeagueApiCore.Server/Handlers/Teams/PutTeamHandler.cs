using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Teams;

public record PutTeamRequest(long TeamId, LeagueUser User, PutTeamModel Model) : IRequest<TeamModel>;

public class PutTeamHandler : TeamsHandlerBase<PutTeamHandler, PutTeamRequest>, 
    IRequestHandler<PutTeamRequest, TeamModel>
{
    public PutTeamHandler(ILogger<PutTeamHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutTeamRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<TeamModel> Handle(PutTeamRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putTeam = await GetTeamEntity(request.TeamId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putTeam = await MapToTeamEntityAsync(request.User, request.Model, putTeam, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getTeam = await MapToTeamModel(request.TeamId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getTeam;
    }
}

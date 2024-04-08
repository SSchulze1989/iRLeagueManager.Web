using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Teams;

public record GetTeamRequest(long teamId) : IRequest<TeamModel>;

public class GetTeamHandler : TeamsHandlerBase<GetTeamHandler, GetTeamRequest>, 
    IRequestHandler<GetTeamRequest, TeamModel>
{
    public GetTeamHandler(ILogger<GetTeamHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetTeamRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<TeamModel> Handle(GetTeamRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getTeam = await MapToTeamModel(request.teamId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getTeam;
    }
}

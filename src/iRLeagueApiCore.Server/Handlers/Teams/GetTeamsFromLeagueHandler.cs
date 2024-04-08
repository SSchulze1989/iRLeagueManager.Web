using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Teams;

public record GetTeamsFromLeagueRequest() : IRequest<IEnumerable<TeamModel>>;

public sealed class GetTeamsFromLeagueHandler : TeamsHandlerBase<GetTeamsFromLeagueHandler, GetTeamsFromLeagueRequest>,
    IRequestHandler<GetTeamsFromLeagueRequest, IEnumerable<TeamModel>>
{
    public GetTeamsFromLeagueHandler(ILogger<GetTeamsFromLeagueHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetTeamsFromLeagueRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<TeamModel>> Handle(GetTeamsFromLeagueRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getTeams = await MapToTeamModels(cancellationToken);
        return getTeams;
    }

    private async Task<IEnumerable<TeamModel>> MapToTeamModels(CancellationToken cancellationToken)
    {
        return await dbContext.Teams
            .Select(MapToTeamModelExpression)
            .ToListAsync(cancellationToken);
    }
}

using iRLeagueApiCore.Common.Models.Standings;

namespace iRLeagueApiCore.Server.Handlers.Standings;

public record GetStandingsFromEventRequest(long EventId) : IRequest<IEnumerable<StandingsModel>>;

public sealed class GetStandingsFromEventHandler : StandingsHandlerBase<GetStandingsFromEventHandler, GetStandingsFromEventRequest>,
    IRequestHandler<GetStandingsFromEventRequest, IEnumerable<StandingsModel>>
{
    public GetStandingsFromEventHandler(ILogger<GetStandingsFromEventHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetStandingsFromEventRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<StandingsModel>> Handle(GetStandingsFromEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getStandings = await MapToStandingModelFromEventAsync(request.EventId, cancellationToken);
        return getStandings;
    }

    private async Task<IEnumerable<StandingsModel>> MapToStandingModelFromEventAsync(long eventId, CancellationToken cancellationToken)
    {
        var standings = await dbContext.Standings
            .Where(x => x.EventId == eventId)
            .Select(MapToStandingModelExpression)
            .ToListAsync(cancellationToken);
        if (standings.Any() == false)
        {
            return standings;
        }
        standings = (await AlignStandingResultRows(standings.Select(x => x.SeasonId).First(), standings, cancellationToken)).ToList();
        return standings;
    }
}

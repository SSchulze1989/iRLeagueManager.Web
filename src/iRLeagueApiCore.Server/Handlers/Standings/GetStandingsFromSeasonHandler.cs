using iRLeagueApiCore.Common.Models.Standings;

namespace iRLeagueApiCore.Server.Handlers.Standings;

public record GetStandingsFromSeasonRequest(long SeasonId) : IRequest<IEnumerable<StandingsModel>>;

public sealed class GetStandingsFromSeasonHandler : StandingsHandlerBase<GetStandingsFromSeasonHandler, GetStandingsFromSeasonRequest>,
    IRequestHandler<GetStandingsFromSeasonRequest, IEnumerable<StandingsModel>>
{
    public GetStandingsFromSeasonHandler(ILogger<GetStandingsFromSeasonHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetStandingsFromSeasonRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<StandingsModel>> Handle(GetStandingsFromSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getStandings = await MapToStandingModelFromSeasonAsync(request.SeasonId, cancellationToken);
        return getStandings;
    }

    private async Task<IEnumerable<StandingsModel>> MapToStandingModelFromSeasonAsync(long seasonId, CancellationToken cancellationToken)
    {
        var lastEventWithStandingsId = await dbContext.Standings
            .Where(x => x.SeasonId == seasonId)
            .OrderByDescending(x => x.Event.Date)
            .Select(x => x.EventId)
            .FirstOrDefaultAsync(cancellationToken);
        var standings = await dbContext.Standings
            .Where(x => x.EventId == lastEventWithStandingsId)
            .Select(MapToStandingModelExpression)
            .ToListAsync(cancellationToken);
        if (standings.Any() == false)
        {
            return standings;
        }
        standings = (await AlignStandingResultRows(seasonId, standings, cancellationToken)).ToList();
        return standings;
    }
}

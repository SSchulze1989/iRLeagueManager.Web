using iRLeagueApiCore.Common.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Championships;

public record GetChampSeasonsFromChampionshipRequest(long ChampionshipId) : IRequest<IEnumerable<ChampSeasonModel>>;

public sealed class GetChampSeasonFromChampionshipHandler : ChampSeasonHandlerBase<GetChampSeasonFromChampionshipHandler, GetChampSeasonsFromChampionshipRequest>,
    IRequestHandler<GetChampSeasonsFromChampionshipRequest, IEnumerable<ChampSeasonModel>>
{
    public GetChampSeasonFromChampionshipHandler(ILogger<GetChampSeasonFromChampionshipHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<GetChampSeasonsFromChampionshipRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ChampSeasonModel>> Handle(GetChampSeasonsFromChampionshipRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getChampSeasons = await MapToChampSeasonModelsFromChampionshipAsync(request.ChampionshipId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getChampSeasons;
    }

    private async Task<IEnumerable<ChampSeasonModel>> MapToChampSeasonModelsFromChampionshipAsync(long championshipId, CancellationToken cancellationToken)
    {
        return await ChampSeasonsQuery()
            .Where(x => x.ChampionshipId == championshipId)
            .OrderByDescending(x => x.Season.SeasonStart)
            .Select(MapToChampSeasonModelExpression)
            .ToListAsync(cancellationToken);
    }
}

using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Championships;

public record GetChampionshipsFromLeagueRequest() : IRequest<IEnumerable<ChampionshipModel>>;

public sealed class GetChampionshipsFromLeagueHandler : ChampionshipHandlerBase<GetChampionshipsFromLeagueHandler, GetChampionshipsFromLeagueRequest>, 
    IRequestHandler<GetChampionshipsFromLeagueRequest, IEnumerable<ChampionshipModel>>
{
    public GetChampionshipsFromLeagueHandler(ILogger<GetChampionshipsFromLeagueHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<GetChampionshipsFromLeagueRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ChampionshipModel>> Handle(GetChampionshipsFromLeagueRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getChampionships = await MapToChampionshipModelsFromLeagueAsync(cancellationToken);
        return getChampionships;
    }

    private async Task<IEnumerable<ChampionshipModel>> MapToChampionshipModelsFromLeagueAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Championships
            .Where(x => x.IsArchived == false)
            .Select(MapToChampionshipModelExpression)
            .ToListAsync(cancellationToken);
    }
}

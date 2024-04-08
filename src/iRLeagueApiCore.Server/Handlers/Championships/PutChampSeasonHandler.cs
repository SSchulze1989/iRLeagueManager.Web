using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Championships;

public record PutChampSeasonRequest(long ChampSeasonId, LeagueUser User, PutChampSeasonModel Model) : IRequest<ChampSeasonModel>;

public sealed class PutChampSeasonHandler : ChampSeasonHandlerBase<PutChampSeasonHandler, PutChampSeasonRequest>,
    IRequestHandler<PutChampSeasonRequest, ChampSeasonModel>
{
    public PutChampSeasonHandler(ILogger<PutChampSeasonHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutChampSeasonRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<ChampSeasonModel> Handle(PutChampSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putChampSeason = await GetChampSeasonEntityAsync(request.ChampSeasonId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putChampSeason = await MapToChampSeasonEntityAsync(request.User, request.Model, putChampSeason, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getChampSeason = await MapToChampSeasonModel(putChampSeason.ChampSeasonId, cancellationToken)
            ?? throw new InvalidOperationException("Updated resource not found");
        return getChampSeason;
    }
}

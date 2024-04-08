using iRLeagueApiCore.Services.ResultService.Extensions;

namespace iRLeagueApiCore.Server.Handlers.Seasons;

public record DeleteSeasonRequest(long SeasonId) : IRequest;

public sealed class DeleteSeasonHandler : SeasonHandlerBase<DeleteSeasonHandler, DeleteSeasonRequest>, IRequestHandler<DeleteSeasonRequest>
{
    public DeleteSeasonHandler(ILogger<DeleteSeasonHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteSeasonRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        await DeleteSeasonEntity(request.SeasonId, cancellationToken);
        await dbContext.SaveChangesAsync();
        return Unit.Value;
    }

    private async Task DeleteSeasonEntity(long seasonId, CancellationToken cancellationToken)
    {
        var deleteSeason = await dbContext.Seasons
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.ResultConfigurations)
                    .ThenInclude(x => x.PointFilters)
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.ResultConfigurations)
                    .ThenInclude(x => x.ResultFilters)
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.DefaultResultConfig)
            .SingleOrDefaultAsync(x => x.SeasonId == seasonId)
            ?? throw new ResourceNotFoundException();
        dbContext.RemoveRange(deleteSeason.ChampSeasons.SelectMany(x => x.ResultConfigurations).SelectMany(x => x.PointFilters));
        dbContext.RemoveRange(deleteSeason.ChampSeasons.SelectMany(x => x.ResultConfigurations).SelectMany(x => x.ResultFilters));
        dbContext.RemoveRange(deleteSeason.ChampSeasons.SelectMany(x => x.ResultConfigurations));
        deleteSeason.ChampSeasons.ForEach(x => x.DefaultResultConfig = null);
        dbContext.Seasons.Remove(deleteSeason);
    }
}

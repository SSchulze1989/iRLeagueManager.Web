using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using System.Linq.Expressions;

namespace iRLeagueApiCore.Server.Handlers.Championships;

public class ChampionshipHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public ChampionshipHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<ChampionshipEntity?> GetChampionshipEntityAsync(long championshipId, CancellationToken cancellationToken)
    {
        return await dbContext.Championships
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.Filters)
            .Where(x => x.ChampionshipId == championshipId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<ChampionshipEntity> MapToChampionshipEntityAsync(LeagueUser user, PostChampionshipModel postModel, ChampionshipEntity target, 
        CancellationToken cancellationToken)
    {
        target.Name = postModel.Name;
        target.DisplayName = postModel.DisplayName;
        UpdateVersionEntity(user, target);
        return await Task.FromResult(target);
    }

    protected virtual async Task<ChampionshipModel?> MapToChampionshipModelAsync(long championshipId, CancellationToken cancellationToken)
    {
        return await dbContext.Championships
            .Where(x => x.ChampionshipId == championshipId)
            .Where(x => x.IsArchived == false)
            .Select(MapToChampionshipModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual Expression<Func<ChampionshipEntity, ChampionshipModel>> MapToChampionshipModelExpression => championship => new()
    {
        ChampionshipId = championship.ChampionshipId,
        Name= championship.Name,
        DisplayName = championship.DisplayName,
        Seasons = championship.ChampSeasons
            .Where(x => x.IsActive)
            .Select(champSeason => new ChampSeasonInfoModel()
        {
            ChampionshipId = championship.ChampionshipId,
            ChampionshipName = championship.Name,
            ChampSeasonId = champSeason.ChampSeasonId,
            SeasonId = champSeason.SeasonId,
            SeasonName = champSeason.Season.SeasonName,
        }).ToList(),
    };
}

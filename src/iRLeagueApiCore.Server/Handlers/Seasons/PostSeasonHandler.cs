using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Seasons;

public record PostSeasonRequest(LeagueUser User, PostSeasonModel Model) : IRequest<SeasonModel>;

public sealed class PostSeasonHandler : SeasonHandlerBase<PostSeasonHandler, PostSeasonRequest>, IRequestHandler<PostSeasonRequest, SeasonModel>
{
    public PostSeasonHandler(ILogger<PostSeasonHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PostSeasonRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<SeasonModel> Handle(PostSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);

        var postSeason = await CreateSeasonEntity(request.User, dbContext.LeagueProvider.LeagueId, cancellationToken);
        await MapToSeasonEntityAsync(request.User, request.Model, postSeason, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getSeason = await MapToGetSeasonModel(postSeason.SeasonId, cancellationToken)
            ?? throw new InvalidOperationException($"Creating season {request.Model.SeasonName} failed");
        return getSeason;
    }

    private async Task<SeasonEntity> CreateSeasonEntity(LeagueUser user, long leagueId, CancellationToken cancellationToken = default)
    {
        var league = await dbContext.Leagues
            .SingleOrDefaultAsync(x => x.Id == leagueId, cancellationToken) ?? throw new ResourceNotFoundException();
        var seasonEntity = CreateVersionEntity(user, new SeasonEntity());
        league.Seasons.Add(seasonEntity);
        return seasonEntity;
    }
}

using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Seasons;

public record GetSeasonsRequest() : IRequest<IEnumerable<SeasonModel>>;

public sealed class GetSeasonsHandler : SeasonHandlerBase<GetSeasonsHandler, GetSeasonsRequest>, IRequestHandler<GetSeasonsRequest, IEnumerable<SeasonModel>>
{
    public GetSeasonsHandler(ILogger<GetSeasonsHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetSeasonsRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<SeasonModel>> Handle(GetSeasonsRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getSeasons = await GetSeasonsEntityAsync(cancellationToken);
        if (getSeasons.Count() == 0)
        {
            throw new ResourceNotFoundException();
        }
        return getSeasons;
    }

    private async Task<IEnumerable<SeasonModel>> GetSeasonsEntityAsync(CancellationToken cancellationToken)
    {
        return (await dbContext.Seasons
            .Select(MapToGetSeasonModelExpression)
            .ToListAsync(cancellationToken))
            .OrderBy(x => x.SeasonStart);
    }
}

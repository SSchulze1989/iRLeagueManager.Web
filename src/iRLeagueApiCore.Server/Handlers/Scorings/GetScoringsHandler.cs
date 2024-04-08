using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Scorings;

public record GetScoringsRequest() : IRequest<IEnumerable<ScoringModel>>;

public sealed class GetScoringsHandler : ScoringHandlerBase<GetScoringsHandler, GetScoringsRequest>, IRequestHandler<GetScoringsRequest, IEnumerable<ScoringModel>>
{
    public GetScoringsHandler(ILogger<GetScoringsHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetScoringsRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ScoringModel>> Handle(GetScoringsRequest request, CancellationToken cancellationToken = default)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        return await MapToGetScoringModelsAsync(cancellationToken);
    }

    private async Task<IEnumerable<ScoringModel>> MapToGetScoringModelsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Scorings
            .Select(MapToGetScoringModelExpression)
            .ToListAsync(cancellationToken);
    }
}

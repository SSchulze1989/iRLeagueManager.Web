using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Scorings;

public record PutScoringRequest(long ScoringId, LeagueUser User, PutScoringModel Model) : IRequest<ScoringModel>;

public sealed class PutScoringHandler : ScoringHandlerBase<PutScoringHandler, PutScoringRequest>, IRequestHandler<PutScoringRequest, ScoringModel>
{
    public PutScoringHandler(ILogger<PutScoringHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutScoringRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<ScoringModel> Handle(PutScoringRequest request, CancellationToken cancellationToken = default)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putScoring = await GetScoringEntityAsync(request.ScoringId) ?? throw new ResourceNotFoundException();
        await MapToScoringEntityAsync(request.User, request.Model, putScoring, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getScoring = await MapToGetScoringModelAsync(putScoring.ScoringId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getScoring;
    }
}

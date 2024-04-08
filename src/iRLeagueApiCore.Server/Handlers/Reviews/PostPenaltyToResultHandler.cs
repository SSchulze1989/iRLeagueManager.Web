using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record PostPenaltyToResultRequest(long ResultId, long ScoredResultRowId, PostPenaltyModel Model) : IRequest<PenaltyModel>;
public class PostPenaltyToResultHandler : ReviewsHandlerBase<PostPenaltyToResultHandler, PostPenaltyToResultRequest>, 
    IRequestHandler<PostPenaltyToResultRequest, PenaltyModel>
{
    public PostPenaltyToResultHandler(ILogger<PostPenaltyToResultHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PostPenaltyToResultRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<PenaltyModel> Handle(PostPenaltyToResultRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postPenalty = await CreatePenalty(request.ResultId, request.ScoredResultRowId, cancellationToken);
        postPenalty = await MapToAddPenaltyEntity(request.Model, postPenalty, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getPenaltyEntity = await dbContext.AddPenaltys
            .Where(x => x.AddPenaltyId == postPenalty.AddPenaltyId)
            .FirstAsync(cancellationToken);
        var getPenalty = await MapToAddPenaltyModel(postPenalty.AddPenaltyId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getPenalty;
    }

    private async Task<AddPenaltyEntity> CreatePenalty(long resultId, long scoredResultRowId, CancellationToken cancellationToken)
    {
        var result = await dbContext.ScoredSessionResults
            .Include(x => x.ScoredResultRows)
                .ThenInclude(x => x.Member)
            .Where(x => x.SessionResultId == resultId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ResourceNotFoundException();
        var resultRow = result.ScoredResultRows
            .Where(x => x.ScoredResultRowId == scoredResultRowId)
            .FirstOrDefault()
            ?? throw new ResourceNotFoundException();

        var penalty = new AddPenaltyEntity()
        {
            LeagueId = result.LeagueId,
            ScoredResultRow = resultRow,
        };
        dbContext.AddPenaltys.Add(penalty);

        return penalty;
    }
}

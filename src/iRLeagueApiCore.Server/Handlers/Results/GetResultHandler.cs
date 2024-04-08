using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record GetResultRequest(long ResultId) : IRequest<EventResultModel>;

public sealed class GetResultHandler : ResultHandlerBase<GetResultHandler, GetResultRequest>, IRequestHandler<GetResultRequest, EventResultModel>
{
    public GetResultHandler(ILogger<GetResultHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetResultRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<EventResultModel> Handle(GetResultRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getResult = await MapToEventResultModelAsync(request.ResultId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getResult;
    }

    private async Task<EventResultModel?> MapToEventResultModelAsync(long resultId, CancellationToken cancellationToken)
    {
        return await dbContext.ScoredEventResults
            .Where(x => x.ResultId == resultId)
            .Select(MapToEventResultModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

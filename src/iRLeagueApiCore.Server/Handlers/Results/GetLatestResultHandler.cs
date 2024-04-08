using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record GetLatestResultRequest() : IRequest<IEnumerable<EventResultModel>>;

public class GetLatestResultHandler : ResultHandlerBase<GetLatestResultHandler, GetLatestResultRequest>, IRequestHandler<GetLatestResultRequest, IEnumerable<EventResultModel>>
{
    public GetLatestResultHandler(ILogger<GetLatestResultHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetLatestResultRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<EventResultModel>> Handle(GetLatestResultRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var latestEventWithResult = await GetLatestEventWithResult(cancellationToken);
        if (latestEventWithResult is null)
        {
            return Array.Empty<EventResultModel>();
        }
        var getResult = await MapToGetResultModelsFromEventAsync(latestEventWithResult.EventId, cancellationToken);
        return getResult;
    }

    private async Task<EventEntity?> GetLatestEventWithResult(CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .Where(x => x.ScoredEventResults.Any())
            .OrderBy(x => x.Date)
            .LastOrDefaultAsync(cancellationToken);
    }
}

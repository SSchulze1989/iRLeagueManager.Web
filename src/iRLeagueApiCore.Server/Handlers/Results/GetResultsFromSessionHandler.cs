using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record GetResultsFromEventRequest(long EventId) : IRequest<IEnumerable<EventResultModel>>;

public sealed class GetResultsFromSessionHandler : ResultHandlerBase<GetResultsFromSessionHandler, GetResultsFromEventRequest>,
    IRequestHandler<GetResultsFromEventRequest, IEnumerable<EventResultModel>>
{
    public GetResultsFromSessionHandler(ILogger<GetResultsFromSessionHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetResultsFromEventRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<EventResultModel>> Handle(GetResultsFromEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getResults = await MapToGetResultModelsFromEventAsync(request.EventId, cancellationToken);
        if (getResults.Count() == 0)
        {
            throw new ResourceNotFoundException();
        }
        return getResults;
    }
}

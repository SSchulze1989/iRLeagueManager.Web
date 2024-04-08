using iRLeagueApiCore.Common.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record GetResultConfigsFromLeagueRequest() : IRequest<IEnumerable<ResultConfigModel>>;

public sealed class GetResultConfigsFromLeagueHandler : ResultConfigHandlerBase<GetResultConfigsFromLeagueHandler, GetResultConfigsFromLeagueRequest>,
    IRequestHandler<GetResultConfigsFromLeagueRequest, IEnumerable<ResultConfigModel>>
{
    public GetResultConfigsFromLeagueHandler(ILogger<GetResultConfigsFromLeagueHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<GetResultConfigsFromLeagueRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ResultConfigModel>> Handle(GetResultConfigsFromLeagueRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getResults = await MapToGetResultConfigsFromLeagueAsync(cancellationToken);
        return getResults;
    }

    private async Task<IEnumerable<ResultConfigModel>> MapToGetResultConfigsFromLeagueAsync(CancellationToken cancellationToken)
    {
        return await dbContext.ResultConfigurations
            .Select(MapToResultConfigModelExpression)
            .ToListAsync(cancellationToken);
    }
}

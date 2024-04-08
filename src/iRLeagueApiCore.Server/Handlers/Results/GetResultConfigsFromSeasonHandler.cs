using iRLeagueApiCore.Common.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record GetResultConfigsFromSeasonRequest(long SeasonId) : IRequest<IEnumerable<ResultConfigModel>>;

public class GetResultConfigsFromSeasonHandler : ResultConfigHandlerBase<GetResultConfigsFromSeasonHandler, GetResultConfigsFromSeasonRequest>,
    IRequestHandler<GetResultConfigsFromSeasonRequest, IEnumerable<ResultConfigModel>>
{
    public GetResultConfigsFromSeasonHandler(ILogger<GetResultConfigsFromSeasonHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<GetResultConfigsFromSeasonRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ResultConfigModel>> Handle(GetResultConfigsFromSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var configs = await GetResultConfigsFromSeasonAsync(request.SeasonId, cancellationToken);
        return configs;
    }

    private async Task<IEnumerable<ResultConfigModel>> GetResultConfigsFromSeasonAsync(long seasonId, CancellationToken cancellationToken)
    {
        return await dbContext.ChampSeasons
            .Where(x => x.SeasonId == seasonId)
            .Where(x => x.IsActive)
            .SelectMany(x => x.ResultConfigurations)
            .Select(MapToResultConfigModelExpression)
            .ToListAsync(cancellationToken);
    }
}

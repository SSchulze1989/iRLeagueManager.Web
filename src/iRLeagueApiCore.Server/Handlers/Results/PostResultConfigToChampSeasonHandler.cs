using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record PostResultConfigToChampSeasonRequest(long ChampSeasonId, LeagueUser User, PostResultConfigModel Model) : IRequest<ResultConfigModel>;

public sealed class PostResultConfigToChampSeasonHandler : ResultConfigHandlerBase<PostResultConfigToChampSeasonHandler, PostResultConfigToChampSeasonRequest>,
    IRequestHandler<PostResultConfigToChampSeasonRequest, ResultConfigModel>
{
    public PostResultConfigToChampSeasonHandler(ILogger<PostResultConfigToChampSeasonHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<PostResultConfigToChampSeasonRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<ResultConfigModel> Handle(PostResultConfigToChampSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postResultConfig = await CreateResultConfigEntity(request.ChampSeasonId, request.User, cancellationToken);
        postResultConfig = await MapToResultConfigEntityAsync(request.User, request.Model, postResultConfig, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getResultConfig = await MapToResultConfigModel(postResultConfig.ResultConfigId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getResultConfig;
    }

    private async Task<ResultConfigurationEntity> CreateResultConfigEntity(long champSeasonId, LeagueUser user, CancellationToken cancellationToken)
    {
        var champSeason = await dbContext.ChampSeasons
            .Where(x => x.ChampSeasonId == champSeasonId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ResourceNotFoundException();
        var resultConfig = CreateVersionEntity(user, new ResultConfigurationEntity());
        champSeason.ResultConfigurations.Add(resultConfig);
        return await Task.FromResult(resultConfig);
    }
}

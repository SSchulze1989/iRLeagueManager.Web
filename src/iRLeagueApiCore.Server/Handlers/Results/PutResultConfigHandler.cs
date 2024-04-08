using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record PutResultConfigRequest(long ResultConfigId, LeagueUser User, PutResultConfigModel Model) : IRequest<ResultConfigModel>;

public sealed class PutResultConfigHandler : ResultConfigHandlerBase<PutResultConfigHandler, PutResultConfigRequest>,
    IRequestHandler<PutResultConfigRequest, ResultConfigModel>
{
    public PutResultConfigHandler(ILogger<PutResultConfigHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<PutResultConfigRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<ResultConfigModel> Handle(PutResultConfigRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putResultConfig = await GetResultConfigEntity(request.ResultConfigId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putResultConfig = await MapToResultConfigEntityAsync(request.User, request.Model, putResultConfig, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getResultConfig = await MapToResultConfigModel(putResultConfig.ResultConfigId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getResultConfig;
    }
}

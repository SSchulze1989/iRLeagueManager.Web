using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Results;

public record DeleteResultConfigRequest(long ResultConfigId) : IRequest;

public sealed class DeleteResultConfigHandler : ResultConfigHandlerBase<DeleteResultConfigHandler, DeleteResultConfigRequest>,
    IRequestHandler<DeleteResultConfigRequest>
{
    public DeleteResultConfigHandler(ILogger<DeleteResultConfigHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<DeleteResultConfigRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteResultConfigRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deleteResultConfig = await GetResultConfigEntity(request.ResultConfigId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        dbContext.ResultConfigurations.Remove(deleteResultConfig);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

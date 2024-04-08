namespace iRLeagueApiCore.Server.Handlers.Scorings;

public record DeletePointRuleRequest(long PointRuleId) : IRequest;

public sealed class DeletePointRuleHandler : PointRuleHandlerBase<DeletePointRuleHandler, DeletePointRuleRequest>,
    IRequestHandler<DeletePointRuleRequest, Unit>
{
    public DeletePointRuleHandler(ILogger<DeletePointRuleHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeletePointRuleRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeletePointRuleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deletePointRule = await GetPointRuleEntityAsync(request.PointRuleId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        dbContext.PointRules.Remove(deletePointRule);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

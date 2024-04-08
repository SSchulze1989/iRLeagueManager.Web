using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Scorings;

public record PutPointRuleRequest(long PointRuleId, LeagueUser User, PutPointRuleModel Model) : IRequest<PointRuleModel>;

public sealed class PutPointRuleHandler : PointRuleHandlerBase<PutPointRuleHandler, PutPointRuleRequest>,
    IRequestHandler<PutPointRuleRequest, PointRuleModel>
{
    public PutPointRuleHandler(ILogger<PutPointRuleHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutPointRuleRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<PointRuleModel> Handle(PutPointRuleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putPointRule = await GetPointRuleEntityAsync(request.PointRuleId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putPointRule = await MapToPointRuleEntityAsync(request.User, request.Model, putPointRule, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getPointRule = await MapToPointRuleModel(putPointRule.PointRuleId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getPointRule;
    }
}

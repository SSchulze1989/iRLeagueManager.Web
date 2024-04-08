using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Scorings;

public record GetPointRuleRequest(long PointRuleId) : IRequest<PointRuleModel>;

public sealed class GetPointRuleHandler : PointRuleHandlerBase<GetPointRuleHandler, GetPointRuleRequest>,
    IRequestHandler<GetPointRuleRequest, PointRuleModel>
{
    public GetPointRuleHandler(ILogger<GetPointRuleHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetPointRuleRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<PointRuleModel> Handle(GetPointRuleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getPointRule = await MapToPointRuleModel(request.PointRuleId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getPointRule;
    }
}

using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Scorings;

public record PostPointRuleRequest(LeagueUser User, PostPointRuleModel Model) : IRequest<PointRuleModel>;

public sealed class PostPointRuleHandler : PointRuleHandlerBase<PostPointRuleHandler, PostPointRuleRequest>,
    IRequestHandler<PostPointRuleRequest, PointRuleModel>
{
    public PostPointRuleHandler(ILogger<PostPointRuleHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<PostPointRuleRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<PointRuleModel> Handle(PostPointRuleRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postPointRule = await CreatePointRuleEntityAsync(request.User, cancellationToken);
        postPointRule = await MapToPointRuleEntityAsync(request.User, request.Model, postPointRule, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getPointRule = await MapToPointRuleModel(postPointRule.PointRuleId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getPointRule;
    }

    private async Task<PointRuleEntity> CreatePointRuleEntityAsync(LeagueUser user, CancellationToken cancellationToken)
    {
        var league = await GetCurrentLeagueEntityAsync(cancellationToken)
            ?? throw new ResourceNotFoundException();
        var pointRule = CreateVersionEntity(user, new PointRuleEntity());
        league.PointRules.Add(pointRule);
        return pointRule;
    }
}

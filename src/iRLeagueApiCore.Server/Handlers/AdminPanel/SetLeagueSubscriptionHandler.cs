using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Handlers.Leagues;
using iRLeagueApiCore.Server.Models.Payments;

namespace iRLeagueApiCore.Server.Handlers.AdminPanel;

public record SetLeagueSubscriptionRequest(long LeagueId, SetLeagueSubscriptionModel Model) : IRequest<LeagueModel>;

public class SetLeagueSubscriptionHandler : LeagueHandlerBase<SetLeagueSubscriptionHandler, SetLeagueSubscriptionRequest>,
    IRequestHandler<SetLeagueSubscriptionRequest, LeagueModel>
{
    public SetLeagueSubscriptionHandler(ILogger<SetLeagueSubscriptionHandler> logger, LeagueDbContext dbContext, 
        IEnumerable<IValidator<SetLeagueSubscriptionRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<LeagueModel> Handle(SetLeagueSubscriptionRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var league = await GetLeagueEntityAsync(request.LeagueId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        league.Subscription = request.Model.Status;
        league.Expires = request.Model.Expires;
        await dbContext.SaveChangesAsync(cancellationToken);
        var getLeague = await MapToGetLeagueModelAsync(request.LeagueId, true, cancellationToken);
        return getLeague!;
    }
}

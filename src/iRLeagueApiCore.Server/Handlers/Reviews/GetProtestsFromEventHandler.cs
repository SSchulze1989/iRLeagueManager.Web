using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;

namespace iRLeagueApiCore.Server.Handlers.Reviews;

public record GetProtestsFromEventRequest(LeagueUser User, long EventId) : IRequest<IEnumerable<ProtestModel>>;

public class GetProtestsFromEventHandler : ProtestsHandlerBase<GetProtestsFromEventHandler, GetProtestsFromEventRequest>, IRequestHandler<GetProtestsFromEventRequest, 
    IEnumerable<ProtestModel>>
{
    public GetProtestsFromEventHandler(ILogger<GetProtestsFromEventHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetProtestsFromEventRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<ProtestModel>> Handle(GetProtestsFromEventRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var league = await GetCurrentLeagueEntityAsync(cancellationToken)
            ?? throw new ResourceNotFoundException();
        var isSteward = request.User.IsInRole(LeagueRoles.Admin, LeagueRoles.Steward);

        // return unauthorized if protests are not public and user is not a steward
        if (isSteward == false && league.ProtestsPublic == ProtestPublicSetting.Hidden)
        {
            return Array.Empty<ProtestModel>();
        }

        var includeAuthor = GetIncludeName(league, isSteward);
        var getProtests = await MapToProtestModelsFromEvent(request.EventId, includeAuthor, cancellationToken);
        return getProtests;
    }

    private bool GetIncludeName(LeagueEntity league, bool isSteward) => league.ProtestsPublic switch
    {
        ProtestPublicSetting.WithProtester => true,
        _ => isSteward
    };

private async Task<IEnumerable<ProtestModel>> MapToProtestModelsFromEvent(long eventId, bool includeAuthor, CancellationToken cancellationToken)
    {
        return await dbContext.Protests
            .Where(x => x.Session.EventId == eventId)
            .Select(MapToProtestModelExpression(includeAuthor))
            .ToListAsync(cancellationToken);
    }
}

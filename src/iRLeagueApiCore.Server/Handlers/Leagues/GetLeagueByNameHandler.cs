using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Leagues;

public sealed record GetLeagueByNameRequest(string LeagueName, bool IncludeSubscriptionDetails) : IRequest<LeagueModel>;

public sealed class GetLeagueByNameHandler : LeagueHandlerBase<GetLeagueByNameHandler, GetLeagueByNameRequest>,
    IRequestHandler<GetLeagueByNameRequest, LeagueModel>
{
    public GetLeagueByNameHandler(ILogger<GetLeagueByNameHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetLeagueByNameRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<LeagueModel> Handle(GetLeagueByNameRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getLeague = await MapToLeagueModelFromName(request.LeagueName, request.IncludeSubscriptionDetails, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getLeague;
    }

    private async Task<LeagueModel?> MapToLeagueModelFromName(string leagueName, bool includeSubscriptionDetails, CancellationToken cancellationToken)
    {
        return await dbContext.Leagues
            .IgnoreQueryFilters()
            .Where(x => x.Name == leagueName)
            .Select(MapToGetLeagueModelExpression(includeSubscriptionDetails))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

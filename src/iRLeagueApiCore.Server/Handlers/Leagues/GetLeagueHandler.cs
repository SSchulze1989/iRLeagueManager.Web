using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Leagues;

public record GetLeagueRequest(long LeagueId, bool IncludeSubscriptionDetails) : IRequest<LeagueModel>;

public sealed class GetLeagueHandler : LeagueHandlerBase<GetLeagueHandler, GetLeagueRequest>, IRequestHandler<GetLeagueRequest, LeagueModel>
{
    public GetLeagueHandler(ILogger<GetLeagueHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetLeagueRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<LeagueModel> Handle(GetLeagueRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getLeague = await MapToGetLeagueModelAsync(request.LeagueId, request.IncludeSubscriptionDetails, cancellationToken) ?? throw new ResourceNotFoundException();
        return getLeague;
    }
}

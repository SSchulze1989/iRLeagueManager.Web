using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Leagues;

public record PutLeagueRequest(long LeagueId, LeagueUser User, PutLeagueModel Model) : IRequest<LeagueModel>;

public sealed class PutLeagueHandler : LeagueHandlerBase<PutLeagueHandler, PutLeagueRequest>, IRequestHandler<PutLeagueRequest, LeagueModel>
{
    public PutLeagueHandler(ILogger<PutLeagueHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutLeagueRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<LeagueModel> Handle(PutLeagueRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putLeague = await GetLeagueEntityAsync(request.LeagueId, cancellationToken) ?? throw new ResourceNotFoundException();
        MapToLeagueEntity(request.LeagueId, request.User, request.Model, putLeague);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getLeague = await MapToGetLeagueModelAsync(putLeague.Id, true, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getLeague;
    }
}

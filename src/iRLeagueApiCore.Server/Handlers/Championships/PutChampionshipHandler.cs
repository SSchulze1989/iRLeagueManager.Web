using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Championships;

public record PutChampionshipRequest(long ChampionshipId, LeagueUser User, PutChampionshipModel Model) : IRequest<ChampionshipModel>;

public sealed class PutChampionshipHandler : ChampionshipHandlerBase<PutChampionshipHandler, PutChampionshipRequest>, 
    IRequestHandler<PutChampionshipRequest, ChampionshipModel>
{
    public PutChampionshipHandler(ILogger<PutChampionshipHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutChampionshipRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<ChampionshipModel> Handle(PutChampionshipRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putChampionship = await GetChampionshipEntityAsync(request.ChampionshipId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        putChampionship = await MapToChampionshipEntityAsync(request.User, request.Model, putChampionship, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var getChampionship = await MapToChampionshipModelAsync(request.ChampionshipId, cancellationToken)
            ?? throw new InvalidOperationException("Updated resource was not found");
        return getChampionship;
    }
}

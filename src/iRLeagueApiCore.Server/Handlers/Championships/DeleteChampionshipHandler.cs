namespace iRLeagueApiCore.Server.Handlers.Championships;

public record DeleteChampionshipRequest(long ChampionshipId) : IRequest;

public class DeleteChampionshipHandler : ChampionshipHandlerBase<DeleteChampionshipHandler, DeleteChampionshipRequest>,
    IRequestHandler<DeleteChampionshipRequest, Unit>
{
    public DeleteChampionshipHandler(ILogger<DeleteChampionshipHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<DeleteChampionshipRequest>> validators) : 
        base(logger, dbContext, validators)
    {
    }

    public async Task<Unit> Handle(DeleteChampionshipRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var deleteChampionship = await GetChampionshipEntityAsync(request.ChampionshipId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        // only archive championship instead of deleting
        deleteChampionship.IsArchived = true;
        foreach(var champSeason in deleteChampionship.ChampSeasons)
        {
            champSeason.IsActive = false;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

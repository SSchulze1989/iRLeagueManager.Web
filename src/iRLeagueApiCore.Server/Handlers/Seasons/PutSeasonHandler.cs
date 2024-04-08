using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;

namespace iRLeagueApiCore.Server.Handlers.Seasons;

public record PutSeasonRequest(LeagueUser User, long SeasonId, PutSeasonModel Model) : IRequest<SeasonModel>;

public sealed class PutSeasonHandler : SeasonHandlerBase<PutSeasonHandler, PutSeasonRequest>, IRequestHandler<PutSeasonRequest, SeasonModel>
{
    public PutSeasonHandler(ILogger<PutSeasonHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PutSeasonRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<SeasonModel> Handle(PutSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var putSeason = await GetSeasonEntityAsync(request.SeasonId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        await MapToSeasonEntityAsync(request.User, request.Model, putSeason, cancellationToken);
        await dbContext.SaveChangesAsync();
        var getSeason = await MapToGetSeasonModel(request.SeasonId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource was not found");
        return getSeason;
    }
}

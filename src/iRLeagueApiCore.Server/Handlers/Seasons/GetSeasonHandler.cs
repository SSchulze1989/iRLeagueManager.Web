using iRLeagueApiCore.Common.Models;

namespace iRLeagueApiCore.Server.Handlers.Seasons;

public record GetSeasonRequest(long SeasonId) : IRequest<SeasonModel>;

public sealed class GetSeasonHandler : SeasonHandlerBase<GetSeasonHandler, GetSeasonRequest>, IRequestHandler<GetSeasonRequest, SeasonModel>
{
    public GetSeasonHandler(ILogger<GetSeasonHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<GetSeasonRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    public async Task<SeasonModel> Handle(GetSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getSeason = await MapToGetSeasonModel(request.SeasonId, cancellationToken)
            ?? throw new ResourceNotFoundException();
        return getSeason;
    }
}

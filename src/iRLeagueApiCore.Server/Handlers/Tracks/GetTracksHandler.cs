using iRLeagueApiCore.Common.Models.Tracks;

namespace iRLeagueApiCore.Server.Handlers.Tracks;

public record GetTracksRequest() : IRequest<IEnumerable<TrackGroupModel>>;

public sealed class GetTracksHandler : TracksHandlerBase<GetTracksHandler, GetTracksRequest>,
    IRequestHandler<GetTracksRequest, IEnumerable<TrackGroupModel>>
{
    public GetTracksHandler(ILogger<GetTracksHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<GetTracksRequest>> validators) : base(logger, dbContext, validators)
    {
    }

    public async Task<IEnumerable<TrackGroupModel>> Handle(GetTracksRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var getTracks = await MapToTrackGroupModels(cancellationToken);
        return getTracks;
    }
}

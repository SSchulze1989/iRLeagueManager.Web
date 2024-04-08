using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Server.Models;
using iRLeagueApiCore.TrackImport.Models;
using iRLeagueApiCore.TrackImport.Service;

namespace iRLeagueApiCore.Server.Handlers.Tracks;

public record ImportTracksCommand(IracingAuthModel Model) : IRequest;

public sealed class ImportTracksHandler : HandlerBase<ImportTracksHandler, ImportTracksCommand>,
    IRequestHandler<ImportTracksCommand, Unit>
{
    private readonly TrackImportService trackImportService;

    public ImportTracksHandler(ILogger<ImportTracksHandler> logger, LeagueDbContext dbContext,
        IEnumerable<IValidator<ImportTracksCommand>> validators, TrackImportService trackImportService) : base(logger, dbContext, validators)
    {
        this.trackImportService = trackImportService;
    }

    public async Task<Unit> Handle(ImportTracksCommand request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        await AuthenticateService(request.Model, trackImportService, cancellationToken);
        var importTracks = await trackImportService.GetTracksData(cancellationToken);
        await UpdateTracksInDatabase(dbContext, importTracks, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private static async Task<bool> AuthenticateService(IracingAuthModel authData, TrackImportService trackImportService, CancellationToken cancellationToken)
    {
        var userName = authData.UserName;
        var password = authData.Password;
        var result = await trackImportService.Authenticate(userName, password, cancellationToken);
        return true;
    }

    private async Task UpdateTracksInDatabase(LeagueDbContext dbContext, IEnumerable<TrackImportModel> importTracks, CancellationToken cancellationToken)
    {
        foreach (var importTrack in importTracks)
        {
            var trackConfig = dbContext.TrackConfigs.Local
                .FirstOrDefault(x => x.TrackId == importTrack.track_id)
                ?? await dbContext.TrackConfigs
                .Include(x => x.TrackGroup)
                .FirstOrDefaultAsync(x => x.TrackId == importTrack.track_id, cancellationToken);
            var trackGroup = trackConfig?.TrackGroup
                ?? dbContext.TrackGroups.Local.FirstOrDefault(x => x.TrackName == importTrack.track_name)
                ?? await dbContext.TrackGroups.FirstOrDefaultAsync(x => x.TrackName == importTrack.track_name, cancellationToken);
            if (trackGroup == null)
            {
                _logger.LogInformation("Track {TrackName} does not exist and will be created", importTrack.track_name);
                trackGroup = new TrackGroupEntity()
                {
                    Location = importTrack.location,
                    TrackName = importTrack.track_name,
                };
                dbContext.TrackGroups.Add(trackGroup);
            }
            if (trackConfig == null)
            {
                _logger.LogInformation("TrackConfig with id: {TrackId} does not exist and will be created", importTrack.track_id);
                trackConfig = new TrackConfigEntity()
                {
                    TrackGroup = trackGroup,
                    TrackId = importTrack.track_id,
                };
                dbContext.TrackConfigs.Add(trackConfig);
            }

            trackConfig = MapToTrackConfigEntity(importTrack, trackConfig);
            _logger.LogInformation("Updated data for track id: {TrackId}, track: {TrackName}, config: {ConfigName}",
                trackConfig.TrackId, trackConfig.TrackGroup.TrackName, trackConfig.ConfigName);
        }
    }

    private static TrackConfigEntity MapToTrackConfigEntity(TrackImportModel importTrack, TrackConfigEntity entity)
    {
        entity.ConfigName = importTrack.config_name ?? "-";
        entity.ConfigType = GetConfigType(importTrack.track_types?[0].track_type ?? string.Empty);
        entity.HasNightLighting = importTrack.night_lighting;
        entity.LengthKm = importTrack.track_config_length * TrackImportService.m2km;
        entity.Turns = importTrack.corners_per_lap;
        return entity;
    }

    private static ConfigType GetConfigType(string typeString)
    {
        switch (typeString)
        {
            case "road":
                return ConfigType.Road;
            case "oval":
                return ConfigType.Oval;
            case "dirt_oval":
                return ConfigType.DirtOval;
            case "dirt_road":
                return ConfigType.DirtRoad;
            default:
                return ConfigType.Unknown;
        }
    }
}

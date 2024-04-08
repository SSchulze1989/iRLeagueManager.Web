using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueDatabaseCore;
using System.Threading;

namespace iRLeagueApiCore.Server.Handlers.Championships;

public record PostChampSeasonRequest(long ChampionshipId, long SeasonId, LeagueUser User, PostChampSeasonModel Model) : IRequest<ChampSeasonModel>;

public sealed class PostChampSeasonHandler : ChampSeasonHandlerBase<PostChampSeasonHandler, PostChampSeasonRequest>, 
    IRequestHandler<PostChampSeasonRequest, ChampSeasonModel>
{
    public PostChampSeasonHandler(ILogger<PostChampSeasonHandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<PostChampSeasonRequest>> validators) 
        : base(logger, dbContext, validators)
    {
    }

    public async Task<ChampSeasonModel> Handle(PostChampSeasonRequest request, CancellationToken cancellationToken)
    {
        await validators.ValidateAllAndThrowAsync(request, cancellationToken);
        var postChampSeason = await GetChampSeasonEntityAsync(request.ChampionshipId, request.SeasonId, cancellationToken)
            ?? await CreateChampSeasonEntityAsync(request.User, request.ChampionshipId, request.SeasonId, cancellationToken);
        postChampSeason.IsActive = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        var getChampSeason = await MapToChampSeasonModel(postChampSeason.ChampSeasonId, cancellationToken)
            ?? throw new InvalidOperationException("Created resource not found");
        return getChampSeason;
    }

    private async Task<ChampSeasonEntity?> GetChampSeasonEntityAsync(long championshipId, long seasonId, CancellationToken cancellationToken)
    {
        return await dbContext.ChampSeasons
            .Where(x => x.ChampionshipId == championshipId)
            .Where(x => x.SeasonId == seasonId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<ChampSeasonEntity> CreateChampSeasonEntityAsync(LeagueUser user, long championshipId, long seasonId, CancellationToken cancellationToken)
    {
        var championship = await dbContext.Championships
            .Where(x => x.ChampionshipId == championshipId)
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.ResultConfigurations)
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.Season)
                    .ThenInclude(x => x.Schedules)
                        .ThenInclude(x => x.Events)
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.Filters)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ResourceNotFoundException();
        var season = await dbContext.Seasons
            .Include(x => x.ChampSeasons)
                .ThenInclude(x => x.ResultConfigurations)
            .Where(x => x.SeasonId == seasonId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ResourceNotFoundException();
        var champSeason = CreateVersionEntity(user, new ChampSeasonEntity()
        {
            Championship = championship,
            Season = season,
            IsActive = true,
        });
        var prevChampSeason = championship.ChampSeasons
            .Select(x => new {ChampSeason = x, Events = x.Season.Schedules.SelectMany(x => x.Events)})
            .Where(x => x.Events.Any())
            .OrderByDescending(x => x.Events.Select(x => x.Date).Where(x => x < DateTime.UtcNow).Max())
            .Select(x => x.ChampSeason)
            .FirstOrDefault();

        // find previous season settings and copy
        champSeason.StandingConfiguration = await CreateOrCopyStandingConfigEntity(user, prevChampSeason?.StandingConfigId, cancellationToken);
        champSeason.ResultConfigurations = await CopyResultConfigurationEntities(user, champSeason, prevChampSeason?.ResultConfigurations.Select(x => x.ResultConfigId), cancellationToken);
        champSeason.Filters = CopyChampSeasonFilterEntities(user, prevChampSeason?.Filters);
        UpdateVersionEntity(user, champSeason);

        championship.ChampSeasons.Add(champSeason);
        season.ChampSeasons.Add(champSeason);
        //dbContext.ChampSeasons.Add(champSeason);
        return champSeason;
    }

    private ICollection<FilterOptionEntity> CopyChampSeasonFilterEntities(LeagueUser user, ICollection<FilterOptionEntity>? filters)
    {
        var copyFilters = filters?.Select(x => CreateVersionEntity(user, new FilterOptionEntity()
        {
            Conditions = x.Conditions,

        })) ?? new List<FilterOptionEntity>();
        copyFilters.ForEach(x => UpdateVersionEntity(user, x));
        return copyFilters.ToList();
    }

    private async Task<StandingConfigurationEntity> CreateOrCopyStandingConfigEntity(LeagueUser user, long? prevStandingConfigId, CancellationToken cancellationToken)
    {
        var target = CreateVersionEntity(user, new StandingConfigurationEntity()
        {
            LeagueId = dbContext.LeagueProvider.LeagueId,
        });
        var source = await dbContext.StandingConfigurations
            .Where(x => x.StandingConfigId == prevStandingConfigId)
            .FirstOrDefaultAsync(cancellationToken);
        if (source is not null)
        {
            target.Name = source.Name;
            target.ResultKind = source.ResultKind;
            target.UseCombinedResult = source.UseCombinedResult;
            target.WeeksCounted = source.WeeksCounted;
        }
        UpdateVersionEntity(user, target);
        return target;
    }

    private FilterOptionEntity CopyFilterOptionEntity(LeagueUser user, FilterOptionEntity source) 
    {
        var target = CreateVersionEntity(user, new FilterOptionEntity()
        {
            Conditions = source.Conditions,
        });
        UpdateVersionEntity(user, target);
        return target;
    }

    private PointRuleEntity? CopyPointRuleEntity(LeagueUser user, PointRuleEntity? source)
    {
        if (source is null)
        {
            return null;
        }
        var target = CreateVersionEntity(user, new PointRuleEntity()
        {
            League = source.League,
            BonusPoints = source.BonusPoints,
            FinalSortOptions = source.FinalSortOptions,
            MaxPoints = source.MaxPoints,
            Name = source.Name,
            PointDropOff = source.PointDropOff,
            PointsPerPlace = source.PointsPerPlace,
            PointsSortOptions = source.PointsSortOptions,
            AutoPenalties = CopyAutoPenalties(source.AutoPenalties),
        });
        UpdateVersionEntity(user, target);
        return target;
    }

    private static ICollection<AutoPenaltyConfigEntity> CopyAutoPenalties(ICollection<AutoPenaltyConfigEntity> autoPenalties)
    {
        return autoPenalties.Select(x => new AutoPenaltyConfigEntity()
        {
            Conditions = x.Conditions,
            Description = x.Description,
            Points = x.Points,
            Positions = x.Positions,
            Time = x.Time,
            Type = x.Type,
        }).ToList();
    }

    private ScoringEntity CopyScoringEntity(LeagueUser user, ScoringEntity source)
    {
        var target = CreateVersionEntity(user, new ScoringEntity()
        {
            IsCombinedResult = source.IsCombinedResult,
            Index = source.Index,
            Name = source.Name,
            UpdateTeamOnRecalculation = source.UpdateTeamOnRecalculation,
            UseExternalSourcePoints = source.UseExternalSourcePoints,
            UseResultSetTeam = source.UseResultSetTeam,
            MaxResultsPerGroup = source.MaxResultsPerGroup,
            ShowResults = source.ShowResults,
            PointsRule = CopyPointRuleEntity(user, source.PointsRule),
        });
        UpdateVersionEntity(user, target);
        return target;
    }

    private ResultConfigurationEntity CopyResultConfigurationEntity(LeagueUser user, ChampSeasonEntity currentChampSeason, ResultConfigurationEntity source)
    {
        var target = CreateVersionEntity(user, new ResultConfigurationEntity() 
        {
            DisplayName = source.DisplayName,
            Name = source.Name,
            ResultsPerTeam = source.ResultsPerTeam,
            SourceResultConfig = GetSourceResultConfigurationEntity(currentChampSeason, source),
            ResultFilters = source.ResultFilters.Select(x => CopyFilterOptionEntity(user, x)).ToList(),
            PointFilters = source.PointFilters.Select(x => CopyFilterOptionEntity(user, x)).ToList(),
            Scorings = source.Scorings.Select(x => CopyScoringEntity(user, x)).ToList(),
        });
        UpdateVersionEntity(user, target);
        return target;
    }

    private ResultConfigurationEntity? GetSourceResultConfigurationEntity(ChampSeasonEntity currentChampSeason, ResultConfigurationEntity config)
    {
        if (config.SourceResultConfig is null)
        {
            return null;
        }
        var availableConfigs = currentChampSeason.Season.ChampSeasons.SelectMany(x => x.ResultConfigurations);
        var sourceConfig = availableConfigs.FirstOrDefault(x => x.ResultConfigId == config.SourceResultConfigId)
            ?? availableConfigs
                .Where(x => x.Name == config.SourceResultConfig.Name)
                .Where(x => x.ChampSeason.Championship.Name == config.SourceResultConfig.ChampSeason.Championship.Name)
                .FirstOrDefault();
        return sourceConfig;
    }

    private async Task<ICollection<ResultConfigurationEntity>> CopyResultConfigurationEntities(LeagueUser user, ChampSeasonEntity targetChampSeason, IEnumerable<long>? prevResultConfigIds,
        CancellationToken cancellationToken)
    {
        if (prevResultConfigIds is null)
        {
            return new List<ResultConfigurationEntity>();
        }


        var resultConfigs = await dbContext.ResultConfigurations
            .Where(x => prevResultConfigIds.Contains(x.ResultConfigId))
            .Include(x => x.League)
            .Include(x => x.ChampSeason)
                .ThenInclude(x => x.Season)
            .Include(x => x.ChampSeason)
                .ThenInclude(x => x.Championship)
            .Include(x => x.SourceResultConfig)
                .ThenInclude(x => x.ChampSeason)
                    .ThenInclude(x => x.Championship)
            .Include(x => x.PointFilters)
            .Include(x => x.Scorings)
                .ThenInclude(x => x.PointsRule)
                    .ThenInclude(x => x.AutoPenalties)
            .Include(x => x.ResultFilters)
            .ToListAsync(cancellationToken);
        return resultConfigs.Select(x => CopyResultConfigurationEntity(user, targetChampSeason, x)).ToList();
    }

}

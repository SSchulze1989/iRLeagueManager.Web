using Castle.Components.DictionaryAdapter.Xml;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Server.Models;
using iRLeagueDatabaseCore;
using System.Linq.Expressions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace iRLeagueApiCore.Server.Handlers.Results;

public class ResultConfigHandlerBase<THandler, TRequest> : HandlerBase<THandler, TRequest>
{
    public ResultConfigHandlerBase(ILogger<THandler> logger, LeagueDbContext dbContext, IEnumerable<IValidator<TRequest>> validators) :
        base(logger, dbContext, validators)
    {
    }

    protected virtual async Task<ResultConfigurationEntity?> GetResultConfigEntity(long resultConfigId, CancellationToken cancellationToken)
    {
        return await dbContext.ResultConfigurations
            .Include(x => x.ChampSeason)
            .Include(x => x.Scorings)
                .ThenInclude(x => x.PointsRule)
                    .ThenInclude(x => x.AutoPenalties)
            .Include(x => x.SourceResultConfig)
            .Include(x => x.ResultFilters)
            .Include(x => x.PointFilters)
            .Where(x => x.ResultConfigId == resultConfigId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected virtual async Task<ResultConfigurationEntity> MapToResultConfigEntityAsync(LeagueUser user, PostResultConfigModel postResultConfig,
        ResultConfigurationEntity resultConfigEntity, CancellationToken cancellationToken)
    {
        resultConfigEntity.DisplayName = postResultConfig.DisplayName;
        resultConfigEntity.SourceResultConfig = postResultConfig.SourceResultConfig != null
            ? await dbContext.ResultConfigurations
                .FirstOrDefaultAsync(x => x.ResultConfigId == postResultConfig.SourceResultConfig.ResultConfigId, cancellationToken)
            : null;
        resultConfigEntity.Name = postResultConfig.Name;
        resultConfigEntity.ResultsPerTeam = postResultConfig.ResultsPerTeam;
        resultConfigEntity.Scorings = await MapToScoringList(user, postResultConfig.Scorings, resultConfigEntity.Scorings, cancellationToken);
        resultConfigEntity.PointFilters = await MapToFilterOptionListAsync(user, postResultConfig.FiltersForPoints,
            resultConfigEntity.PointFilters, cancellationToken);
        resultConfigEntity.ResultFilters = await MapToFilterOptionListAsync(user, postResultConfig.FiltersForResult,
            resultConfigEntity.ResultFilters, cancellationToken);
        UpdateVersionEntity(user, resultConfigEntity);
        return await Task.FromResult(resultConfigEntity);
    }

    private async Task<ICollection<StandingConfigurationEntity>> MapToStandingConfigListAsync(LeagueUser user, StandingConfigModel? standingConfigModel,
        ICollection<StandingConfigurationEntity> standingConfigurationEntities, CancellationToken cancellationToken)
    {
        if (standingConfigModel is null)
        {
            return Array.Empty<StandingConfigurationEntity>().ToList();
        }

        var standingConfigEntity = standingConfigurationEntities.FirstOrDefault(x => x.StandingConfigId == standingConfigModel.StandingConfigId);
        if (standingConfigEntity is null)
        {
            standingConfigEntity = CreateVersionEntity(user, new StandingConfigurationEntity());
            standingConfigEntity.LeagueId = dbContext.LeagueProvider.LeagueId;
            standingConfigurationEntities.Clear();
            standingConfigurationEntities.Add(standingConfigEntity);
        }
        standingConfigEntity.Name = standingConfigModel.Name;
        standingConfigEntity.ResultKind = standingConfigModel.ResultKind;
        standingConfigEntity.UseCombinedResult = standingConfigModel.UseCombinedResult;
        standingConfigEntity.WeeksCounted = standingConfigModel.WeeksCounted;
        UpdateVersionEntity(user, standingConfigEntity);
        return await Task.FromResult(standingConfigurationEntities);
    }

    private async Task<ICollection<ScoringEntity>> MapToScoringList(LeagueUser user, ICollection<ScoringModel> scoringModels,
        ICollection<ScoringEntity> scoringEntities, CancellationToken cancellationToken)
    {
        // Map votes
        foreach (var scoringModel in scoringModels)
        {
            var scoringEntity = scoringModel.Id == 0 ? null : scoringEntities
                .FirstOrDefault(x => x.ScoringId == scoringModel.Id);
            if (scoringEntity == null)
            {
                scoringEntity = CreateVersionEntity(user, new ScoringEntity());
                scoringEntities.Add(scoringEntity);
                scoringEntity.LeagueId = dbContext.LeagueProvider.LeagueId;
            }
            await MapToScoringEntityAsync(user, scoringModel, scoringEntity, cancellationToken);
        }
        // Delete votes that are no longer in source collection
        var deleteScorings = scoringEntities
            .Where(x => scoringModels.Any(y => y.Id == x.ScoringId) == false);
        foreach (var deleteScoring in deleteScorings)
        {
            dbContext.Remove(deleteScoring);
        }
        return scoringEntities;
    }

    private async Task<ScoringEntity> MapToScoringEntityAsync(LeagueUser user, ScoringModel scoringModel, ScoringEntity scoringEntity,
        CancellationToken cancellationToken)
    {
        scoringEntity.Index = scoringModel.Index;
        scoringEntity.MaxResultsPerGroup = scoringModel.MaxResultsPerGroup;
        scoringEntity.Name = scoringModel.Name;
        scoringEntity.ShowResults = scoringModel.ShowResults;
        scoringEntity.IsCombinedResult = scoringModel.IsCombinedResult;
        scoringEntity.UseResultSetTeam = scoringModel.UseResultSetTeam;
        scoringEntity.UpdateTeamOnRecalculation = scoringModel.UpdateTeamOnRecalculation;
        scoringEntity.PointsRule = scoringModel.PointRule is not null ? await MapToPointRuleEntityAsync(user, scoringModel.PointRule,
            scoringEntity.PointsRule ?? CreateVersionEntity(user, new PointRuleEntity() { LeagueId = dbContext.LeagueProvider.LeagueId }), cancellationToken) : null;
        scoringEntity.UseExternalSourcePoints = scoringModel.UseSourcePoints;
        UpdateVersionEntity(user, scoringEntity);
        return await Task.FromResult(scoringEntity);
    }

    private async Task<PointRuleEntity> MapToPointRuleEntityAsync(LeagueUser user, PointRuleModel pointRuleModel, PointRuleEntity pointRuleEntity,
        CancellationToken cancellationToken)
    {
        pointRuleEntity.RuleType = pointRuleModel.RuleType;
        pointRuleEntity.BonusPoints = pointRuleModel.BonusPoints;
        pointRuleEntity.FinalSortOptions = pointRuleModel.FinalSortOptions;
        pointRuleEntity.MaxPoints = pointRuleModel.MaxPoints;
        pointRuleEntity.Name = pointRuleModel.Name;
        pointRuleEntity.PointDropOff = pointRuleModel.PointDropOff;
        pointRuleEntity.PointsPerPlace = pointRuleModel.PointsPerPlace;
        pointRuleEntity.PointsSortOptions = pointRuleModel.PointsSortOptions;
        pointRuleEntity.Formula = pointRuleModel.Formula;
        pointRuleEntity.AutoPenalties = await MapToAutoPenaltyCollection(pointRuleModel.AutoPenalties, pointRuleEntity.AutoPenalties, cancellationToken);
        UpdateVersionEntity(user, pointRuleEntity);
        return pointRuleEntity;
    }

    private async Task<ICollection<AutoPenaltyConfigEntity>> MapToAutoPenaltyCollection(IEnumerable<AutoPenaltyConfiguration> models, ICollection<AutoPenaltyConfigEntity> entities,
        CancellationToken cancellationToken)
    {
        foreach(var model in models)
        {
            var entity = entities
                .Where(x => x.PenaltyConfigId != 0 && x.PenaltyConfigId == model.PenaltyConfigId)
                .FirstOrDefault();
            if (entity is null)
            {
                entity = new() { LeagueId = dbContext.LeagueProvider.LeagueId };
                entities.Add(entity);
            }
            await MapToAutoPenaltyConfigEntity(model, entity, cancellationToken);
        }
        var delete = entities
            .Where(x => models.Any(y => y.PenaltyConfigId == x.PenaltyConfigId) == false);
        foreach (var deleteEntity in delete)
        {
            dbContext.Remove(deleteEntity);
        }
        return entities;
    }

    private async Task<AutoPenaltyConfigEntity> MapToAutoPenaltyConfigEntity(AutoPenaltyConfiguration model, AutoPenaltyConfigEntity entity,
        CancellationToken cancellationToken)
    {
        entity.Conditions = model.Conditions;
        entity.Description = model.Description;
        entity.Points = model.Points;
        entity.Positions = model.Positions;
        entity.Time = model.Time;
        entity.Type = model.Type;
        return await Task.FromResult(entity);
    }

    private static FilterCondition MapToFilterCondition(FilterConditionModel model) => new()
    {
        ColumnPropertyName = model.ColumnPropertyName,
        Comparator = model.Comparator,
        FilterType = model.FilterType,
        FilterValues = model.FilterValues,
    };

    protected virtual async Task<ResultConfigurationEntity> MapToResultConfigEntityAsync(LeagueUser user, PutResultConfigModel putResultConfig, ResultConfigurationEntity resultConfigEntity, CancellationToken cancellationToken)
    {
        return await MapToResultConfigEntityAsync(user, (PostResultConfigModel)putResultConfig, resultConfigEntity, cancellationToken);
    }

    protected virtual async Task<ResultConfigModel?> MapToResultConfigModel(long resultConfigId, CancellationToken cancellationToken)
    {
        return await dbContext.ResultConfigurations
            .Where(x => x.ResultConfigId == resultConfigId)
            .Select(MapToResultConfigModelExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Expression<Func<ResultConfigurationEntity, ResultConfigModel>> MapToResultConfigModelExpression => resultConfig => new ResultConfigModel()
    {
        LeagueId = resultConfig.LeagueId,
        ResultConfigId = resultConfig.ResultConfigId,
        ChampSeasonId = resultConfig.ChampSeasonId,
        ChampionshipName = resultConfig.ChampSeason.Championship.Name,
        SourceResultConfig = resultConfig.SourceResultConfig != null
            ? new ResultConfigInfoModel()
            {
                ResultConfigId = resultConfig.SourceResultConfig.ResultConfigId,
                ChampSeasonId = resultConfig.SourceResultConfig.ChampSeasonId,
                ChampionshipName = resultConfig.SourceResultConfig.ChampSeason.Championship.Name,
                DisplayName = resultConfig.SourceResultConfig.DisplayName,
                LeagueId = resultConfig.SourceResultConfig.LeagueId,
                Name = resultConfig.SourceResultConfig.Name,
            }
            : null,
        Name = resultConfig.Name,
        DisplayName = resultConfig.DisplayName,
        IsDefaultConfig = resultConfig.ResultConfigId == resultConfig.ChampSeason.DefaultResultConfigId,
        ResultsPerTeam = resultConfig.ResultsPerTeam,
        Scorings = resultConfig.Scorings.Select(scoring => new ScoringModel()
        {
            Id = scoring.ScoringId,
            Index = scoring.Index,
            MaxResultsPerGroup = scoring.MaxResultsPerGroup,
            Name = scoring.Name,
            ShowResults = scoring.ShowResults,
            IsCombinedResult = scoring.IsCombinedResult,
            UpdateTeamOnRecalculation = scoring.UpdateTeamOnRecalculation,
            UseResultSetTeam = scoring.UseResultSetTeam,
            UseSourcePoints = scoring.UseExternalSourcePoints,
            PointRule = scoring.PointsRule != null ? new PointRuleModel()
            {
                RuleType = scoring.PointsRule.RuleType,
                BonusPoints = scoring.PointsRule.BonusPoints,
                FinalSortOptions = scoring.PointsRule.FinalSortOptions,
                LeagueId = scoring.LeagueId,
                MaxPoints = scoring.PointsRule.MaxPoints,
                PointDropOff = scoring.PointsRule.PointDropOff,
                PointRuleId = scoring.PointsRule.PointRuleId,
                PointsPerPlace = scoring.PointsRule.PointsPerPlace.ToList(),
                PointsSortOptions = scoring.PointsRule.PointsSortOptions,
                Name = scoring.PointsRule.Name,
                Formula = scoring.PointsRule.Formula,
                AutoPenalties = scoring.PointsRule.AutoPenalties.Select(penalty => new AutoPenaltyConfiguration()
                {
                    Conditions = penalty.Conditions,
                    Description = penalty.Description,
                    LeagueId = penalty.LeagueId,
                    PenaltyConfigId = penalty.PenaltyConfigId,
                    Points = penalty.Points,
                    Positions = penalty.Positions,
                    Time = penalty.Time,
                    Type = penalty.Type,
                }).ToList(),
            } : null,
        }).OrderBy(x => x.Index).ToList(),
        FiltersForPoints = resultConfig.PointFilters
            .Select(filter => new ResultFilterModel()
            {
                LeagueId = filter.LeagueId,
                FilterOptionId = filter.FilterOptionId,
                Condition = filter.Conditions.FirstOrDefault() ?? new(),
            }).ToList(),
        FiltersForResult = resultConfig.ResultFilters
            .Select(filter => new ResultFilterModel()
            {
                LeagueId = filter.LeagueId,
                FilterOptionId = filter.FilterOptionId,
                Condition = filter.Conditions.FirstOrDefault() ?? new(),
            }).ToList(),
    };

    public Expression<Func<ScoringEntity, ScoringModel>> MapToGetScoringModelExpression => source => new ScoringModel()
    {
        Id = source.ScoringId,
        MaxResultsPerGroup = source.MaxResultsPerGroup,
        Name = source.Name,
        ShowResults = source.ShowResults,
        UpdateTeamOnRecalculation = source.UpdateTeamOnRecalculation,
        UseResultSetTeam = source.UseResultSetTeam,
    };
}

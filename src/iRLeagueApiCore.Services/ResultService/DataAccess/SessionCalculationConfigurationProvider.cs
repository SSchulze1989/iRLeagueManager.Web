using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using iRLeagueDatabaseCore.Models;
using Microsoft.EntityFrameworkCore;

namespace iRLeagueApiCore.Services.ResultService.DataAccess;

internal sealed class SessionCalculationConfigurationProvider : DatabaseAccessBase, ISessionCalculationConfigurationProvider
{
    public SessionCalculationConfigurationProvider(LeagueDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<SessionCalculationConfiguration>> GetConfigurations(EventEntity eventEntity,
        ResultConfigurationEntity? configurationEntity, CancellationToken cancellationToken = default)
    {
        if (configurationEntity == null)
        {
            return await DefaultSessionResultCalculationConfigurations(eventEntity, configurationEntity, cancellationToken);
        }

        return await GetSessionConfigurationsFromEntity(eventEntity, configurationEntity, cancellationToken);
    }

    private async Task<IEnumerable<SessionCalculationConfiguration>> DefaultSessionResultCalculationConfigurations(EventEntity eventEntity,
        ResultConfigurationEntity? configurationEntity, CancellationToken cancellationToken)
    {
        var configId = configurationEntity?.ResultConfigId;
        var sessionResultIds = await dbContext.ScoredSessionResults
            .Where(x => x.ScoredEventResult.EventId == eventEntity.EventId)
            .Where(x => x.ScoredEventResult.ResultConfigId == configId)
            .OrderBy(x => x.SessionNr)
            .Select(x => x.SessionResultId)
            .ToListAsync(cancellationToken);

        var configurations = eventEntity.Sessions
            .OrderBy(x => x.SessionNr)
            .Select((x, i) => new SessionCalculationConfiguration()
            {
                LeagueId = x.LeagueId,
                ScoringId = null,
                SessionResultId = sessionResultIds.ElementAtOrDefault(i),
                SessionId = x.SessionId,
                SessionNr = x.SessionNr,
                UseResultSetTeam = false,
                MaxResultsPerGroup = (configurationEntity?.ResultsPerTeam is null or <= 0) ? int.MaxValue : configurationEntity.ResultsPerTeam,
                Name = x.Name,
                UpdateTeamOnRecalculation = false,
                ResultKind = configurationEntity?.ChampSeason.ResultKind ?? ResultKind.Member,
            });
        return configurations;
    }

    private async Task<IEnumerable<SessionCalculationConfiguration>> GetSessionConfigurationsFromEntity(EventEntity eventEntity,
        ResultConfigurationEntity configurationEntity, CancellationToken cancellationToken)
    {
        var sessionResultIds = await dbContext.ScoredSessionResults
            .Where(x => x.ScoredEventResult.EventId == eventEntity.EventId)
            .Where(x => x.ScoredEventResult.ResultConfigId == configurationEntity.ResultConfigId)
            .OrderBy(x => x.SessionNr)
            .Select(x => x.SessionResultId)
            .ToListAsync(cancellationToken);

        var scorings = configurationEntity.Scorings
            .Where(x => x.IsCombinedResult == false)
            .OrderBy(x => x.Index);
        var raceIndex = scorings.Count() - eventEntity.Sessions.Count(x => x.SessionType == SessionType.Race);
        var sessionConfigurations = new List<SessionCalculationConfiguration>();
        foreach ((var session, var index) in eventEntity.Sessions
            .OrderBy(x => x.SessionNr)
            .Select((x, i) => (x, i)))
        {
            var scoring = session.SessionType != SessionType.Race ? null : scorings.ElementAtOrDefault(raceIndex++);
            var sessionConfiguration = new SessionCalculationConfiguration
            {
                LeagueId = session.LeagueId,
                Name = session.Name,
                SessionId = session.SessionId,
                SessionNr = session.SessionNr,
                ResultKind = configurationEntity.ChampSeason?.ResultKind ?? ResultKind.Member,
                SessionType = session.SessionType,
                SessionResultId = sessionResultIds.ElementAtOrDefault(index)
            };
            sessionConfiguration = MapFromScoringEntity(scoring, configurationEntity, sessionConfiguration);
            sessionConfigurations.Add(sessionConfiguration);
        }
        var combinedScoring = configurationEntity.Scorings.FirstOrDefault(x => x.IsCombinedResult);
        if (combinedScoring != null)
        {
            var sessionConfiguration = new SessionCalculationConfiguration
            {
                LeagueId = configurationEntity.LeagueId,
                Name = combinedScoring.Name,
                SessionId = null,
                SessionNr = 999,
                ResultKind = configurationEntity.ChampSeason?.ResultKind ?? ResultKind.Member,
                IsCombinedResult = true,
                UseExternalSourcePoints = combinedScoring.UseExternalSourcePoints,
                SessionType = SessionType.Race,
                SessionResultId = null,
            };
            sessionConfiguration = MapFromScoringEntity(combinedScoring, configurationEntity, sessionConfiguration, includePointFilters: false);
            sessionConfigurations.Add(sessionConfiguration);
        }
        return sessionConfigurations;
    }

    private static SessionCalculationConfiguration MapFromScoringEntity(ScoringEntity? scoring, ResultConfigurationEntity configurationEntity,
        SessionCalculationConfiguration sessionConfiguration, bool includePointFilters = true)
    {
        sessionConfiguration.PointRule = GetPointRuleFromEntity(scoring?.PointsRule, configurationEntity, includePointFilters: includePointFilters);
        sessionConfiguration.MaxResultsPerGroup = (configurationEntity.ResultsPerTeam <= 0) ? int.MaxValue : configurationEntity.ResultsPerTeam;
        sessionConfiguration.UseResultSetTeam = scoring?.UseResultSetTeam ?? false;
        sessionConfiguration.UpdateTeamOnRecalculation = scoring?.UpdateTeamOnRecalculation ?? false;
        sessionConfiguration.ScoringId = scoring?.ScoringId;

        return sessionConfiguration;
    }

    private static PointRule<ResultRowCalculationResult> GetPointRuleFromEntity(PointRuleEntity? pointsRuleEntity, ResultConfigurationEntity configurationEntity,
        bool includePointFilters = true)
    {
        CalculationPointRuleBase pointRule;

        pointRule = pointsRuleEntity switch
        {
            var rule when rule?.RuleType == PointRuleType.PointList && rule.PointsPerPlace.Any() => new PerPlacePointRule(PointsPerPlaceToDictionary(rule.PointsPerPlace.Select(x => (double)x))),
            var rule when rule?.RuleType == PointRuleType.MaxPointsDropOff && rule.MaxPoints > 0 => new MaxPointRule(rule.MaxPoints, rule.PointDropOff),
            var rule when rule?.RuleType == PointRuleType.Formula && string.IsNullOrEmpty(rule.Formula) == false => new FormulaPointRule(rule.Formula, false),
            _ => new UseResultPointsPointRule()
        };

        pointRule.PointSortOptions = pointsRuleEntity?.PointsSortOptions ?? Array.Empty<SortOptions>();
        pointRule.FinalSortOptions = pointsRuleEntity?.FinalSortOptions ?? Array.Empty<SortOptions>();
        pointRule.BonusPoints = (pointsRuleEntity?.BonusPoints.Select(MapFromBonusPointModel) ?? Array.Empty<BonusPointConfiguration>()).ToList();
        pointRule.ResultFilters = MapFromFilterEntities(configurationEntity.ResultFilters);
        pointRule.ChampSeasonFilters = configurationEntity.ChampSeason != null ? MapFromFilterEntities(configurationEntity.ChampSeason.Filters) : new();
        pointRule.AutoPenalties = (pointsRuleEntity?.AutoPenalties.Select(MapFromAutoPenaltyConfig) ?? Array.Empty<AutoPenaltyConfigurationData>()).ToList();
        if (includePointFilters)
        {
            pointRule.PointFilters = MapFromFilterEntities(configurationEntity.PointFilters);
        }
        else
        {
            pointRule.PointFilters = CreatePointsEligibleFilter();
        }

        // Add normal status filter for Disqualified drivers, when status is not explicitly filtered already.
        var hasStatusPointFilter = configurationEntity.PointFilters.Any(x => x.Conditions.Any(y => y.ColumnPropertyName == nameof(ResultRowCalculationResult.Status)));
        if (hasStatusPointFilter == false)
        {
            var statusFilter = (FilterCombination.And, new ColumnValueRowFilter(
                nameof(ResultRowCalculationResult.Status),
                ComparatorType.IsEqual,
                new[] { ((int)RaceStatus.Disqualified).ToString() },
                MatchedValueAction.Remove) as RowFilter<ResultRowCalculationResult>);
            pointRule.PointFilters = new FilterGroupRowFilter<ResultRowCalculationResult>(
                pointRule.PointFilters.GetFilters().Concat(new[] { statusFilter }));
        }

        return pointRule;
    }

    private static BonusPointConfiguration MapFromBonusPointModel(BonusPointModel bonusPoint)
    {
        var bonusPointConfig = new BonusPointConfiguration()
        {
            Type = bonusPoint.Type,
            Value = (int)bonusPoint.Value,
            Points = (int)bonusPoint.Points,
            Conditions = MapFromFilterEntities(bonusPoint.Conditions, allowForEach: true, overrideFilterCombination: FilterCombination.And),
        };
        return bonusPointConfig;
    }

    private static AutoPenaltyConfigurationData MapFromAutoPenaltyConfig(AutoPenaltyConfigEntity penaltyEntity)
    {
        var penaltyData = new AutoPenaltyConfigurationData
        {
            Conditions = MapFromFilterEntities(penaltyEntity.Conditions, allowForEach: true, overrideFilterCombination: FilterCombination.And),
            Description = penaltyEntity.Description,
            Points = penaltyEntity.Points,
            Positions = penaltyEntity.Positions,
            Time = penaltyEntity.Time,
            Type = penaltyEntity.Type
        };
        return penaltyData;
    }

    private static FilterGroupRowFilter<ResultRowCalculationResult> CreatePointsEligibleFilter() => new(
        new (FilterCombination, RowFilter<ResultRowCalculationResult>)[] {
            (FilterCombination.And, new ColumnValueRowFilter(nameof(ResultRowCalculationResult.PointsEligible), ComparatorType.IsEqual,
                new[] { true.ToString() }, MatchedValueAction.Keep))
        }
    );

    private static FilterGroupRowFilter<ResultRowCalculationResult> MapFromFilterEntities(ICollection<FilterConditionModel> pointFilters,
        bool allowForEach = false, FilterCombination? overrideFilterCombination = null)
    {
        return MapToFilterGroup(pointFilters, allowForEach: allowForEach, overrideFilterCombination: overrideFilterCombination);
    }

    private static FilterGroupRowFilter<ResultRowCalculationResult> MapFromFilterEntities(ICollection<FilterOptionEntity> pointFilters)
    {
        return MapToFilterGroup(pointFilters.Select(x => x.Conditions.FirstOrDefault()));
    }

    private static IReadOnlyDictionary<int, T> PointsPerPlaceToDictionary<T>(IEnumerable<T> points)
    {
        return points
            .Select((x, i) => new { pos = i + 1, value = x })
            .ToDictionary(k => k.pos, v => v.value);
    }

    private static FilterCombination GetFilterCombination(FilterConditionModel? condition, int index)
    {
        if (index == 0 || condition is null)
        {
            return FilterCombination.And;
        }
        return condition.Action switch
        {
            MatchedValueAction.Remove => FilterCombination.And,
            MatchedValueAction.Keep => FilterCombination.Or,
            _ => FilterCombination.And,
        };
    }

    private static FilterGroupRowFilter<ResultRowCalculationResult> MapToFilterGroup(IEnumerable<FilterConditionModel?> filters,
        bool allowForEach = false, FilterCombination? overrideFilterCombination = null)
    {
        var filterCombination = filters
            .Select((x, i) => (overrideFilterCombination ?? GetFilterCombination(x, i), GetRowFilterFromCondition(x, allowForEach: allowForEach)))
            .Where(x => x.Item2 is not null);
        return new(filterCombination!);
    }

    private static RowFilter<ResultRowCalculationResult>? GetRowFilterFromCondition(FilterConditionModel? condition,
        bool allowForEach = false)
    {
        return condition?.FilterType switch
        {
            FilterType.ColumnProperty => new ColumnValueRowFilter(condition.ColumnPropertyName, condition.Comparator,
                condition.FilterValues, condition.Action, allowForEach: allowForEach),
            FilterType.Member => new IdRowFilter<long>(condition.FilterValues, x => x.MemberId.GetValueOrDefault(), condition.Action),
            FilterType.Team => new IdRowFilter<long>(condition.FilterValues, x => x.TeamId.GetValueOrDefault(), condition.Action),
            _ => null,
        };
    }
}

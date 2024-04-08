using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class TeamStandingCalculationService : StandingCalculationServiceBase, ICalculationService<StandingCalculationData, StandingCalculationResult>
{
    public TeamStandingCalculationService(StandingCalculationConfiguration config) : base(config)
    {
    }

    public override Task<StandingCalculationResult> Calculate(StandingCalculationData data)
    {
        var (previousSessionResults, currentSessionResults) = GetPreviousAndCurrentSessionResults(data, config.UseCombinedResult);

        Func<ResultRowCalculationResult, long?> keySelector = x => x.TeamId;
        var previousTeamEventResults = GetGroupedEventResults(previousSessionResults, keySelector);
        var currentTeamEventResult = GetGroupedEventResult(currentSessionResults, keySelector);

        var teamStandingRows = CalculateTeamStandingRows(previousTeamEventResults, currentTeamEventResult);

        // Sort and apply positions standings previous
        teamStandingRows = SortStandingRows(teamStandingRows, x => x.Previous, config.SortOptions)
            .ToList();
        foreach (var (teamStandingRow, position) in teamStandingRows.Select((x, i) => (x, i + 1)))
        {
            teamStandingRow.Previous.Position = position;
        }

        // Sort and apply positions standings current
        teamStandingRows = SortStandingRows(teamStandingRows, x => x.Current, config.SortOptions)
            .ToList();
        var finalStandingRows = new List<StandingRowCalculationResult>();
        foreach (var (teamStandingRow, position) in teamStandingRows.Select((x, i) => (x, i + 1)))
        {
            teamStandingRow.Current.Position = position;
            var final = DiffStandingRows(teamStandingRow.Previous, teamStandingRow.Current);
            finalStandingRows.Add(final);
        }

        var standingResult = new StandingCalculationResult()
        {
            LeagueId = config.LeagueId,
            EventId = config.EventId,
            Name = config.DisplayName,
            SeasonId = config.SeasonId,
            StandingConfigId = config.StandingConfigId,
            StandingRows = finalStandingRows,
            IsTeamStanding = true
        };
        return Task.FromResult(standingResult);
    }

    private List<(long TeamId, StandingRowCalculationResult Previous, StandingRowCalculationResult Current)> CalculateTeamStandingRows(Dictionary<long, IEnumerable<GroupedEventResult<long>>> previousTeamEventResults, Dictionary<long, GroupedEventResult<long>> currentTeamEventResult)
    {
        var teamIds = previousTeamEventResults.Keys.Concat(currentTeamEventResult.Keys).Distinct();
        List<(long TeamId, StandingRowCalculationResult Previous, StandingRowCalculationResult Current)> teamStandingRows = new();
        foreach (var memberId in teamIds)
        {
            // sort by best race points each event 
            var previousEventResults = (previousTeamEventResults.GetValueOrDefault(memberId) ?? Array.Empty<GroupedEventResult<long>>())
                .OrderByDescending(GetEventOrderValue);
            var currentResult = currentTeamEventResult.GetValueOrDefault(memberId);
            var standingRow = new StandingRowCalculationResult();
            var lastResult = currentResult ?? previousEventResults.FirstOrDefault();
            var lastRow = lastResult?.SessionResults.LastOrDefault()?.ResultRow;
            if (lastRow is null)
            {
                continue;
            }
            // static data
            standingRow.MemberId = null;
            standingRow.CarClass = lastRow.CarClass;
            standingRow.ClassId = lastRow.ClassId;
            standingRow.TeamId = lastRow.TeamId;

            // accumulated data
            var previousStandingRow = new StandingRowCalculationResult(standingRow);

            var previousResults = previousEventResults.SelectMany(x => x.SessionResults);
            var countedEventResults = previousEventResults.Take(config.WeeksCounted);
            var countedSessionResults = countedEventResults.SelectMany(x => x.SessionResults);
            previousStandingRow = AccumulateOverallSessionResults(previousStandingRow, previousResults);
            previousStandingRow = AccumulateCountedSessionResults(previousStandingRow, countedSessionResults);
            previousStandingRow = AccumulateTotalPoints(previousStandingRow);
            previousStandingRow = SetScoredResultRows(previousStandingRow, previousResults, countedSessionResults);

            if (currentResult is not null)
            {
                var currentResults = previousEventResults.Concat(new[] { currentResult })
                    .OrderByDescending(GetEventOrderValue);
                var currentMemberSessionResults = currentResults.SelectMany(x => x.SessionResults);
                var currentCountedResults = currentResults.Take(config.WeeksCounted);
                var currentCountedSessionResults = currentCountedResults.SelectMany(x => x.SessionResults);
                standingRow = AccumulateOverallSessionResults(standingRow, currentMemberSessionResults);
                standingRow = AccumulateCountedSessionResults(standingRow, currentCountedSessionResults);
                standingRow = AccumulateTotalPoints(standingRow);
                standingRow = SetScoredResultRows(standingRow, currentMemberSessionResults, currentCountedSessionResults);
            }
            else
            {
                standingRow = previousStandingRow;
            }

            teamStandingRows.Add((memberId, previousStandingRow, standingRow));
        }

        return teamStandingRows;
    }
}

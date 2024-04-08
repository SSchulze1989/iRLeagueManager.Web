using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal sealed class MemberStandingCalculationService : StandingCalculationServiceBase, ICalculationService<StandingCalculationData, StandingCalculationResult>
{
    public MemberStandingCalculationService(StandingCalculationConfiguration config) : base(config)
    {
    }

    public override Task<StandingCalculationResult> Calculate(StandingCalculationData data)
    {
        var (previousSessionResults, currentSessionResults) = GetPreviousAndCurrentSessionResults(data, config.UseCombinedResult);

        static long? keySelector(ResultRowCalculationResult x) => x.MemberId;
        var previousMemberEventResults = GetGroupedEventResults(previousSessionResults, keySelector);
        var currentMemberEventResult = GetGroupedEventResult(currentSessionResults, keySelector);

        var memberStandingRows = CalculateMemberStandingRows(previousMemberEventResults, currentMemberEventResult);

        // Sort and apply positions standings previous
        memberStandingRows = SortStandingRows(memberStandingRows, x => x.previous, config.SortOptions)
            .ToList();
        foreach (var (memberStandingRow, position) in memberStandingRows.Select((x, i) => (x, i + 1)))
        {
            memberStandingRow.previous.Position = position;
        }

        // Sort and apply positions standings current
        memberStandingRows = SortStandingRows(memberStandingRows, x => x.current, config.SortOptions)
            .ToList();

        var finalStandingRows = new List<StandingRowCalculationResult>();
        foreach (var (memberStandingRow, position) in memberStandingRows.Select((x, i) => (x, i + 1)))
        {
            memberStandingRow.current.Position = position;
            var final = DiffStandingRows(memberStandingRow.previous, memberStandingRow.current);
            finalStandingRows.Add(final);
        }

        var standingResult = new StandingCalculationResult()
        {
            LeagueId = config.LeagueId,
            EventId = config.EventId,
            Name = config.DisplayName,
            SeasonId = config.SeasonId,
            ChampSeasonId = config.ChampSeasonId,
            StandingConfigId = config.StandingConfigId,
            StandingRows = finalStandingRows
        };
        return Task.FromResult(standingResult);
    }

    private IEnumerable<(long memberId, StandingRowCalculationResult previous, StandingRowCalculationResult current)> CalculateMemberStandingRows(
        Dictionary<long, IEnumerable<GroupedEventResult<long>>> previousMemberResults,
        Dictionary<long, GroupedEventResult<long>> currentMemberResult)
    {
        var memberIds = previousMemberResults.Keys.Concat(currentMemberResult.Keys).Distinct();
        List<(long memberId, StandingRowCalculationResult previous, StandingRowCalculationResult current)> memberStandingRows = new();
        foreach (var memberId in memberIds)
        {
            // sort by best race points each event 
            var previousEventResults = (previousMemberResults.GetValueOrDefault(memberId) ?? Array.Empty<GroupedEventResult<long>>())
                .OrderByDescending(GetEventOrderValue);
            var currentResult = currentMemberResult.GetValueOrDefault(memberId);
            var standingRow = new StandingRowCalculationResult();
            var lastResult = currentResult ?? previousEventResults.FirstOrDefault();
            var lastRow = lastResult?.SessionResults.LastOrDefault()?.ResultRow;
            if (lastRow is null)
            {
                continue;
            }
            // static data
            standingRow.MemberId = lastRow.MemberId;
            standingRow.CarClass = lastRow.CarClass;
            standingRow.ClassId = lastRow.ClassId;
            standingRow.TeamId = lastRow.TeamId;
            standingRow.StartIrating = lastRow.SeasonStartIrating;
            standingRow.LastIrating = lastRow.NewIrating;

            // accumulated data
            var previousStandingRow = new StandingRowCalculationResult(standingRow);

            var previousSessionResults = previousEventResults.SelectMany(x => x.SessionResults);
            var countedEventResults = previousEventResults.Take(config.WeeksCounted);
            var countedSessionResults = countedEventResults.SelectMany(x => x.SessionResults);
            previousStandingRow = AccumulateOverallSessionResults(previousStandingRow, previousSessionResults);
            previousStandingRow = AccumulateCountedSessionResults(previousStandingRow, countedSessionResults);
            previousStandingRow = AccumulateTotalPoints(previousStandingRow);
            previousStandingRow = SetScoredResultRows(previousStandingRow, previousSessionResults, countedSessionResults);

            if (currentResult is not null)
            {
                var currentSessionResults = previousEventResults.Concat(new[] { currentResult })
                    .OrderByDescending(GetEventOrderValue);
                var currentMemberSessionResults = currentSessionResults.SelectMany(x => x.SessionResults);
                var currentCountedResults = currentSessionResults.Take(config.WeeksCounted);
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

            memberStandingRows.Add((memberId, previousStandingRow, standingRow));
        }

        return memberStandingRows;
    }
}

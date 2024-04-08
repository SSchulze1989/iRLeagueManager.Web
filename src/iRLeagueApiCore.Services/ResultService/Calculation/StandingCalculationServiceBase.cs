using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal abstract class StandingCalculationServiceBase : ICalculationService<StandingCalculationData, StandingCalculationResult>
{
    protected readonly StandingCalculationConfiguration config;

    public StandingCalculationServiceBase(StandingCalculationConfiguration config)
    {
        this.config = config;
    }

    protected (IEnumerable<EventSessionResults> PreviousResults, EventSessionResults CurrentResult) GetPreviousAndCurrentSessionResults(
        StandingCalculationData data, bool useCombinedResult)
    {
        IEnumerable<EventSessionResults> previousResults;
        EventSessionResults currentResult;
        if (config.UseCombinedResult)
        {
            previousResults = data.PreviousEventResults
                .Select(eventResult => new EventSessionResults(eventResult, eventResult.SessionResults.OrderBy(x => x.SessionNr).TakeLast(1)));
            currentResult = new EventSessionResults(data.CurrentEventResult, data.CurrentEventResult.SessionResults.OrderBy(x => x.SessionNr).TakeLast(1));
        }
        else
        {
            previousResults = data.PreviousEventResults
                .Select(eventResult => new EventSessionResults(eventResult, eventResult.SessionResults.Where(x => x.Name != "Practice" && x.Name != "Qualifying")));
            currentResult = new EventSessionResults(data.CurrentEventResult,
                data.CurrentEventResult.SessionResults.Where(x => x.Name != "Practice" && x.Name != "Qualifying"));
        }
        return (previousResults, currentResult);
    }

    protected Dictionary<T, GroupedEventResult<T>> GetGroupedEventResult<T>(EventSessionResults sessionResults,
        Func<ResultRowCalculationResult, T?> groupBy) where T : struct
    {
        return GetGroupedEventResults(new[] { sessionResults }, groupBy)
            .ToDictionary(k => k.Key, v => v.Value.First());
    }

    protected Dictionary<T, IEnumerable<GroupedEventResult<T>>> GetGroupedEventResults<T>(IEnumerable<EventSessionResults> sessionResults,
        Func<ResultRowCalculationResult, T?> groupBy) where T : struct
    {
        var resultRows = sessionResults
            .SelectMany(result => result.SessionResults
                .SelectMany(sessionResult => sessionResult.ResultRows
                    .Select(resultRow => (result.EventResult, sessionResult, resultRow))));

        // get the previous result rows for each individual driver
        var groupedResultRows = resultRows
            .Where(x => groupBy(x.resultRow) is not null)
            .GroupBy(x => groupBy(x.resultRow)!.Value);

        // get the previous result rows per event
        var groupedEventResults = groupedResultRows
            .Select(x => (key: x.Key, results: x
                .GroupBy(result => result.EventResult, result => new GroupedSessionResultRow<T>(x.Key, result.sessionResult, result.resultRow))
                .Select(result => new GroupedEventResult<T>(x.Key, result.Key, result))))
            .ToDictionary(k => k.key, v => v.results);

        return groupedEventResults;
    }

    public abstract Task<StandingCalculationResult> Calculate(StandingCalculationData data);

    protected static IComparable GetEventOrderValue<T>(GroupedEventResult<T> eventResult) where T : notnull
        => eventResult.SessionResults.Sum(result => result.ResultRow.RacePoints + result.ResultRow.BonusPoints);

    protected StandingRowCalculationResult AccumulateTotalPoints(StandingRowCalculationResult row)
    {
        row.TotalPoints = row.RacePoints - row.PenaltyPoints;
        return row;
    }

    protected static StandingRowCalculationResult AccumulateCountedSessionResults(StandingRowCalculationResult standingRow,
        IEnumerable<GroupedSessionResultRow<long>> results)
    {
        if (results.None())
        {
            return standingRow;
        }

        standingRow.RacePoints += (int)results.Sum(x => x.ResultRow.RacePoints + x.ResultRow.BonusPoints);
        standingRow.RacesCounted += results.Count();

        return standingRow;
    }

    protected static StandingRowCalculationResult AccumulateOverallSessionResults(StandingRowCalculationResult standingRow,
        IEnumerable<GroupedSessionResultRow<long>> results)
    {
        if (results.None())
        {
            return standingRow;
        }

        // accumulate rows
        foreach (var resultRow in results)
        {
            var sessionResult = resultRow.SessionResult;
            var row = resultRow.ResultRow;
            standingRow.CompletedLaps += (int)row.CompletedLaps;
            standingRow.FastestLaps += sessionResult.FastestLapDriverMemberId == standingRow.MemberId ? 1 : 0;
            standingRow.Incidents += (int)row.Incidents;
            standingRow.LeadLaps += (int)row.LeadLaps;
            standingRow.PenaltyPoints += (int)row.PenaltyPoints;
            standingRow.PolePositions += (int)row.StartPosition == 1 ? 1 : 0;
            standingRow.Top10 += row.FinalPosition <= 10 ? 1 : 0;
            standingRow.Top5 += row.FinalPosition <= 5 ? 1 : 0;
            standingRow.Top3 += row.FinalPosition <= 3 ? 1 : 0;
            standingRow.Wins += row.FinalPosition == 1 ? 1 : 0;
            standingRow.Races += 1;
            standingRow.RacesScored += row.PointsEligible ? 1 : 0;
            standingRow.RacesInPoints += row.RacePoints > 0 ? 1 : 0;
        }

        return standingRow;
    }

    protected static StandingRowCalculationResult SetScoredResultRows(StandingRowCalculationResult standingRow, IEnumerable<GroupedSessionResultRow<long>> allResults,
        IEnumerable<GroupedSessionResultRow<long>> countedResults)
    {
        standingRow.ResultRows.Clear();
        foreach (var resultRow in allResults)
        {
            var standingResultRow = resultRow.ResultRow;
            standingResultRow.IsScored = countedResults.Contains(resultRow);
            standingRow.ResultRows.Add(standingResultRow);
        }
        return standingRow;
    }

    protected static IEnumerable<T> SortStandingRows<T>(IEnumerable<T> rows, Func<T, StandingRowCalculationResult> standingRowSelector, IEnumerable<SortOptions> sortOptions)
    {
        foreach(var sortOption in sortOptions.Reverse())
        {
            rows = rows.OrderBy(x => sortOption.GetStandingSortingValue<StandingRowCalculationResult>()(standingRowSelector(x)));
        }
        return rows;
    }

    protected static StandingRowCalculationResult DiffStandingRows(StandingRowCalculationResult previous, StandingRowCalculationResult current)
    {
        if (previous == current)
        {
            return current;
        }
        var diff = current;

        diff.CompletedLapsChange = current.CompletedLaps - previous.CompletedLaps;
        diff.FastestLapsChange = current.FastestLaps - previous.FastestLaps;
        diff.IncidentsChange = current.Incidents - previous.Incidents;
        diff.LeadLapsChange = current.LeadLaps - previous.LeadLaps;
        diff.PenaltyPointsChange = current.PenaltyPoints - previous.PenaltyPoints;
        diff.PolePositionsChange = current.PolePositions - previous.PolePositions;
        diff.PositionChange = -(current.Position - previous.Position);
        diff.RacePointsChange = current.RacePoints - previous.RacePoints;
        diff.TotalPointsChange = current.TotalPoints - previous.TotalPoints;
        diff.WinsChange = current.Wins - previous.Wins;

        return diff;
    }

    protected record GroupedSessionResultRow<T>(T TeamId, SessionCalculationResult SessionResult, ResultRowCalculationResult ResultRow);

    protected record GroupedEventResult<T>(T TeamId, EventCalculationResult EventResult, IEnumerable<GroupedSessionResultRow<T>> SessionResults);

    protected record EventSessionResults(EventCalculationResult EventResult, IEnumerable<SessionCalculationResult> SessionResults);
}

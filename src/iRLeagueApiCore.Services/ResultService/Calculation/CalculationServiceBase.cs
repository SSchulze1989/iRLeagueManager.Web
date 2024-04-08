using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Services.ResultService.Extensions;
using iRLeagueApiCore.Services.ResultService.Models;
using System.ComponentModel;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

abstract internal class CalculationServiceBase : ICalculationService<SessionCalculationData, SessionCalculationResult>
{
    public abstract Task<SessionCalculationResult> Calculate(SessionCalculationData data);

    protected static IEnumerable<ResultRowCalculationResult> ApplyPoints(IEnumerable<ResultRowCalculationResult> rows, PointRule<ResultRowCalculationResult> pointRule,
        SessionCalculationData data)
    {
        rows = pointRule.GetChampSeasonFilters().FilterRows(rows);
        rows = pointRule.GetResultFilters().FilterRows(rows);
        rows = CalculateCompletedPct(rows);
        rows = CalculateIntervals(rows);
        rows = CalculateAutoPenalties(rows, pointRule.GetAutoPenalties());
        ApplyAddPenaltyDsq(rows);
        ApplyAddPenaltyTimes(rows);
        rows = pointRule.SortForPoints(rows);
        rows = ApplyAddPenaltyPositions(rows);

        IEnumerable<ResultRowCalculationResult> pointRows = rows.ToList();
        // Filter for points only
        pointRows = pointRule.GetPointFilters().FilterRows(pointRows);

        // Calculation
        pointRule.ApplyPoints(data, pointRows.ToList());
        // remove points from filtered rows and set points eligible 
        pointRows.ForEach(x => x.PointsEligible  = true);
        rows.Except(pointRows)
            .ForEach(x => {
                x.RacePoints = 0;
                x.PointsEligible = false;
            });

        IEnumerable<ResultRowCalculationResult> finalRows = rows;
        ApplyAddPenaltyPoints(finalRows);
        ApplyReviewPenalties(finalRows, data.AcceptedReviewVotes);
        ApplyBonusPoints(pointRows, pointRule.GetBonusPoints());
        ApplyTotalPoints(finalRows);
        finalRows = pointRule.SortFinal(finalRows);
        // Set final position
        foreach ((var row, var position) in finalRows.Select((x, i) => (x, i + 1)))
        {
            row.FinalPosition = position;
            row.FinalPositionChange = row.StartPosition - row.FinalPosition;
        }

        return finalRows;
    }

    protected static (TId? id, TimeSpan lap) GetBestLapValue<T, TId>(IEnumerable<T> rows, Func<T, TId?> idSelector, Func<T, TimeSpan> valueSelector)
    {
        return rows
            .Select(row => ((TId? id, TimeSpan lap))(idSelector.Invoke(row), valueSelector.Invoke(row)))
            .Where(row => LapIsValid(row.lap))
            .DefaultIfEmpty()
            .MinBy(row => row.lap);
    }

    protected static IEnumerable<(TId? id, TValue value)> GetBestValues<T, TValue, TId>(IEnumerable<T> rows, Func<T, TValue> valueSelector, Func<T, TId?> idSelector,
        Func<IEnumerable<TValue>, TValue> bestValueFunc, EqualityComparer<TValue>? comparer = default)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        try
        {
            var valueRows = rows.Select(row => ((TId? id, TValue value))(idSelector.Invoke(row), valueSelector.Invoke(row)));
            var bestValue = bestValueFunc.Invoke(valueRows.Select(x => x.value));
            return valueRows.Where(row => comparer.Equals(row.value, bestValue));
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return Array.Empty<(TId? id, TValue value)>();
        }
    }

    protected static TimeSpan GetAverageLapValue<T>(IEnumerable<T> rows, Func<T, TimeSpan> valueSelector, Func<T, double> weightSelector)
    {
        double total = 0;
        TimeSpan lap = TimeSpan.Zero;
        foreach (var row in rows)
        {
            var w = weightSelector(row);
            total += w;
            lap += valueSelector(row) * w;
        }

        return lap / (total > 0 ? total : 1);
    }

    protected static bool LapIsValid(TimeSpan lap)
    {
        return lap > TimeSpan.Zero;
    }

    protected static bool HardChargerEligible(ResultRowCalculationResult row)
    {
        return row.QualifyingTime > TimeSpan.Zero;
    }

    /// <summary>
    /// Group and combine result rows using the given groupBy key selector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rows">rows to combine</param>
    /// <param name="groupBy">selector for the key by which to group the rows</param>
    /// <returns></returns>
    protected static IEnumerable<ResultRowCalculationResult> CombineResults<T>(IEnumerable<ResultRowCalculationResult> rows, Func<ResultRowCalculationResult, T> groupBy)
    {
        var groupedRows = rows.GroupBy(groupBy);
        var combined = new List<ResultRowCalculationResult>();

        foreach (var group in groupedRows.Where(x => x.Any()))
        {
            var row = new ResultRowCalculationResult(group.First());
            foreach (var groupRow in group.Skip(1))
            {
                row.BonusPoints += groupRow.BonusPoints;
                row.CompletedLaps += groupRow.CompletedLaps;
                row.Incidents += groupRow.Incidents;
                row.LeadLaps += groupRow.LeadLaps;
                row.PenaltyPoints += groupRow.PenaltyPoints;
                row.RacePoints += groupRow.RacePoints;
                row.PointsEligible |= groupRow.PointsEligible;
                row.Status = Math.Min(row.Status, groupRow.Status);
            }
            row.StartPosition = group.Last().StartPosition;
            row.AvgLapTime = GetAverageLapValue(group, x => x.AvgLapTime, x => x.CompletedLaps);
            (_, row.FastestLapTime) = GetBestLapValue(group, x => x.MemberId, x => x.FastestLapTime);
            (_, row.QualifyingTime) = GetBestLapValue(group, x => x.MemberId, x => x.QualifyingTime);
            row.FastLapNr = 0;
            var last = group.Last();
            row.NewCpi = last.NewCpi;
            row.NewIrating = last.NewIrating;
            row.NewLicenseLevel = last.NewLicenseLevel;
            row.NewSafetyRating = last.NewSafetyRating;
            combined.Add(row);
        }

        return combined.ToList();
    }

    private static AddPenaltyCalculationData CreateAddPenaltyFromAutoPenalty(ResultRowCalculationResult row, AutoPenaltyConfigurationData autoPenalty, 
        int penaltyMultiplikator)
    {
        var penalty = new AddPenaltyCalculationData()
        {
            MemberId = row.MemberId,
            Points = autoPenalty.Type == PenaltyType.Points ? autoPenalty.Points * penaltyMultiplikator : 0,
            Positions = autoPenalty.Type == PenaltyType.Position ? autoPenalty.Positions * penaltyMultiplikator : 0,
            Time = autoPenalty.Type == PenaltyType.Time ? autoPenalty.Time * penaltyMultiplikator : TimeSpan.Zero,
            Type = autoPenalty.Type,
        };
        return penalty;
    }

    private static IEnumerable<ResultRowCalculationResult> CalculateAutoPenalties(IEnumerable<ResultRowCalculationResult> rows, 
        IEnumerable<AutoPenaltyConfigurationData> autoPenalties)
    {
        foreach(var autoPenalty in autoPenalties)
        {
            var penaltyRows = autoPenalty.Conditions
                .FilterRows(rows);
            var grouped = penaltyRows.GroupBy(x => x);
            foreach(var row in grouped.Where(x => x.Any()))
            {
                row.Key.AddPenalties = row.Key.AddPenalties.Concat(new[] { CreateAddPenaltyFromAutoPenalty(row.Key, autoPenalty, row.Count()) });
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyAddPenaltyDsq(IEnumerable<ResultRowCalculationResult> rows)
    {
        foreach (var row in rows.Where(x => x.AddPenalties.Any(x => x.Type == PenaltyType.Disqualification)))
        {
            foreach (var penalty in row.AddPenalties.Where(x => x.Type == PenaltyType.Disqualification))
            {
                row.Status = (int)RaceStatus.Disqualified;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyAddPenaltyTimes(IEnumerable<ResultRowCalculationResult> rows)
    {
        foreach (var row in rows.Where(x => x.AddPenalties.Any(x => x.Type == PenaltyType.Time)))
        {
            foreach (var penalty in row.AddPenalties.Where(x => x.Type == PenaltyType.Time))
            {
                row.Interval += penalty.Time;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyAddPenaltyPoints(IEnumerable<ResultRowCalculationResult> rows)
    {
        foreach (var row in rows.Where(x => x.AddPenalties.Any(x => x.Type == PenaltyType.Points)))
        {
            foreach (var penalty in row.AddPenalties.Where(x => x.Type == PenaltyType.Points))
            {
                if (penalty.Points < 0)
                {
                    row.BonusPoints -= penalty.Points;
                }
                else
                {
                    row.PenaltyPoints += penalty.Points;
                }
            }
        }
        return rows;
    }

    private static IReadOnlyList<ResultRowCalculationResult> ApplyAddPenaltyPositions(IEnumerable<ResultRowCalculationResult> rows)
    {
        var modRows = rows.ToList();
        foreach (var row in rows.Where(x => x.AddPenalties.Any(x => x.Type == PenaltyType.Position)).Reverse()) // Start from the back to keep order between mutliple drivers with penalties
        {
            var movePositions = row.AddPenalties.Where(x => x.Type == PenaltyType.Position).Sum(x => x.Positions);
            if (movePositions == 0)
            {
                continue;
            }
            var rowIndex = modRows.IndexOf(row);
            modRows.Move(rowIndex, movePositions);
        }
        return modRows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyReviewPenalties(IEnumerable<ResultRowCalculationResult> rows, IEnumerable<AcceptedReviewVoteCalculationData> reviewVotes)
    {
        Func<ResultRowCalculationResult, AcceptedReviewVoteCalculationData, bool> compareIds;
        if (rows.Any(x => x.MemberId != null))
        {
            compareIds = (row, vote) => vote.MemberAtFaultId == row.MemberId;
        }
        else
        {
            compareIds = (row, vote) => vote.TeamAtFaultId == row.TeamId;
        }

        foreach (var row in rows)
        {
            var rowVotes = reviewVotes.Where(vote => compareIds(row, vote));
            foreach (var vote in rowVotes)
            {
                var penalty = new ReviewPenaltyCalculationResult()
                {
                    ReviewId = vote.ReviewId,
                    ReviewVoteId = vote.ReviewVoteId,
                    PenaltyPoints = vote.DefaultPenalty,
                };
                row.ReviewPenalties.Add(penalty);
                if (penalty.PenaltyPoints < 0)
                {
                    row.BonusPoints -= penalty.PenaltyPoints;
                }
                else
                {
                    row.PenaltyPoints += penalty.PenaltyPoints;
                }
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyTotalPoints(IEnumerable<ResultRowCalculationResult> rows)
    {
        foreach (var row in rows)
        {
            row.TotalPoints = row.RacePoints + row.BonusPoints - row.PenaltyPoints;
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyBonusPoints(IEnumerable<ResultRowCalculationResult> rows, IEnumerable<BonusPointConfiguration> BonusPoints)
    {
        if (rows.None())
        {
            return rows;
        }

        var minIncs = rows.Any(x => x.PenaltyPoints == 0) ? rows.Where(x => x.PenaltyPoints == 0).Min(x => x.Incidents) : -1;
        var fastestLapRow = GetBestLapValue(rows, x => x, x => x.FastestLapTime);

        foreach (var bonus in BonusPoints)
        {
            var bonusType = bonus.Type;
            var bonusKeyValue = (int)bonus.Value;
            var bonusPoints = (int)bonus.Points;
            rows = bonusType switch
            {
                BonusPointType.Position => ApplyPositionBonusPoints(rows, bonusKeyValue, bonusPoints),
                BonusPointType.CleanestDriver => ApplyCleanestDriverBonusPoints(rows, bonusPoints),
                BonusPointType.FastestLap => ApplyFastestLapBonusPoints(rows, bonusPoints),
                BonusPointType.QualyPosition => ApplyStartPositionBonusPoints(rows, bonusKeyValue, bonusPoints),
                BonusPointType.MostPositionsGained => ApplyMostPositionsGainedBonusPoints(rows, bonusPoints),
                BonusPointType.MostPositionsLost => ApplyMostPositionsLostBonusPoints(rows, bonusPoints),
                BonusPointType.LeadOneLap => ApplyLeadOneLapBonusPoints(rows, bonusPoints),
                BonusPointType.LeadMostLaps => ApplyLeadMostLapsBonusPoints(rows, bonusPoints),
                BonusPointType.NoIncidents => ApplyNoIncidentsBonusPoints(rows, bonusPoints),
                BonusPointType.FastestAverageLap => ApplyFastestAverageLapBonusPoints(rows, bonusPoints),
                BonusPointType.Custom => ApplyCustomBonusPoints(rows, bonus.Conditions, bonusPoints),
                _ => rows,
            };
        }

        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyCustomBonusPoints(IEnumerable<ResultRowCalculationResult> rows, FilterGroupRowFilter<ResultRowCalculationResult> conditions,
        int bonusPoints)
    {
        var bonusRows = conditions.FilterRows(rows);
        foreach (var row in bonusRows)
        {
            row.BonusPoints += bonusPoints;
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyFastestAverageLapBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        var (row, lapTime) = GetBestLapValue(rows, x => x, x => x.AvgLapTime);
        if (row is not null)
        {
            row.BonusPoints += points;
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyNoIncidentsBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        foreach(var row in rows)
        {
            if (row.Incidents == 0)
            {
                row.BonusPoints += points;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyLeadMostLapsBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        var mostLapsLead = rows.Max(x => x.LeadLaps);
        foreach(var row in rows)
        {
            if (row.LeadLaps == mostLapsLead)
            {
                row.BonusPoints += points;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyLeadOneLapBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        foreach(var row in rows)
        {
            if (row.LeadLaps > 0)
            {
                row.BonusPoints += points;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyMostPositionsLostBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        var mostPositionsLost = rows.Max(x => x.PositionChange);
        foreach (var row in rows)
        {
            if (row.PositionChange == mostPositionsLost)
            {
                row.BonusPoints += points;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyMostPositionsGainedBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        var mostPositionsGained = rows.Min(x => x.PositionChange);
        foreach(var row in rows)
        {
            if (row.PositionChange == mostPositionsGained)
            {
                row.BonusPoints += points;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyPositionBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int position, int points)
    {
        foreach (var row in rows)
        {
            if (row.FinalPosition == position)
            {
                row.BonusPoints += points;
            }
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyCleanestDriverBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        static bool condition(ResultRowCalculationResult x) => x.PenaltyPoints == 0;
        var minIncRows = GetBestValues(rows.Where(condition), x => x.Incidents, x => x, x => x.Min())
            .Select(x => x.id)
            .NotNull();
        foreach (var row in minIncRows)
        {
            row.BonusPoints += points;
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyFastestLapBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int points)
    {
        var (row, lapTime) = GetBestLapValue(rows, x => x, x => x.FastestLapTime);
        if (row is not null)
        {
            row.BonusPoints += points;
        }
        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> ApplyStartPositionBonusPoints(IEnumerable<ResultRowCalculationResult> rows, int position, int points)
    {
        foreach (var row in rows)
        {
            if (row.StartPosition == position)
            {
                row.BonusPoints += points;
            }
        }
        return rows;
    }

    protected static IEnumerable<ResultRowCalculationResult> CalculateCompletedPct(IEnumerable<ResultRowCalculationResult> rows)
    {
        var laps = rows.MaxOrDefault(x => x.CompletedLaps);
        if (laps == 0)
        {
            return rows;
        }

        foreach (var row in rows)
        {
            row.CompletedPct = row.CompletedLaps / laps;
        }

        return rows;
    }

    private static IEnumerable<ResultRowCalculationResult> CalculateIntervals(IEnumerable<ResultRowCalculationResult> rows)
    {
        int totalLaps = (int)rows.MaxOrDefault(x => x.CompletedLaps);
        foreach (var row in rows)
        {
            if (row.Interval.Days > 0)
            {
                row.Interval = TimeSpan.FromDays(totalLaps - row.CompletedLaps);
            }
        }
        return rows;
    }
}

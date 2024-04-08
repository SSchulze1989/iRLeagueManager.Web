using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Models;
using MySqlX.XDevAPI.Relational;
using System.Runtime.CompilerServices;

namespace iRLeagueApiCore.Services.ResultService.Extensions;

internal static class SortOptionsExtensions
{
    public static Func<T, object> GetSortingValue<T>(this SortOptions sortOption) where T : ResultRowCalculationData
    {
        return sortOption switch
        {
            SortOptions.BonusPtsAsc => row => row.BonusPoints,
            SortOptions.BonusPtsDesc => row => -row.BonusPoints,
            SortOptions.ComplLapsAsc => row => row.CompletedLaps,
            SortOptions.ComplLapsDesc => row => -row.CompletedLaps,
            SortOptions.FastLapAsc => row => GetLapTimeSortValue(row.FastestLapTime),
            SortOptions.FastLapDesc => row => -GetLapTimeSortValue(row.FastestLapTime),
            SortOptions.IncsAsc => row => row.Incidents,
            SortOptions.IncsDesc => row => -row.Incidents,
            SortOptions.IntvlAsc => row => row.Interval,
            SortOptions.IntvlDesc => row => -row.Interval,
            SortOptions.LeadLapsAsc => row => row.LeadLaps,
            SortOptions.LeadLapsDesc => row => -row.LeadLaps,
            SortOptions.PenPtsAsc => row => row.PenaltyPoints,
            SortOptions.PenPtsDesc => row => -row.PenaltyPoints,
            SortOptions.PosAsc => row => row.FinishPosition,
            SortOptions.PosDesc => row => -row.FinishPosition,
            SortOptions.QualLapAsc => row => GetLapTimeSortValue(row.QualifyingTime),
            SortOptions.QualLapDesc => row => -GetLapTimeSortValue(row.QualifyingTime),
            SortOptions.RacePtsAsc => row => row.RacePoints,
            SortOptions.RacePtsDesc => row => -row.RacePoints,
            SortOptions.StartPosAsc => row => row.StartPosition,
            SortOptions.StartPosDesc => row => -row.StartPosition,
            SortOptions.TotalPtsAsc => row => row.TotalPoints,
            SortOptions.TotalPtsDesc => row => -row.TotalPoints,
            SortOptions.TotalPtsWoBonusAsc => row => row.RacePoints - row.PenaltyPoints,
            SortOptions.TotalPtsWoBonusDesc => row => -(row.RacePoints - row.PenaltyPoints),
            SortOptions.TotalPtsWoPenaltyAsc => row => row.RacePoints + row.BonusPoints,
            SortOptions.TotalPtsWoPenaltyDesc => row => -(row.RacePoints + row.BonusPoints),
            _ => row => 0,
        };
    }

    public static Func<T, object> GetStandingSortingValue<T>(this SortOptions sortOption) where T : StandingRowCalculationResult
    {
        return sortOption switch
        {
            SortOptions.ComplLapsAsc => row => row.CompletedLaps,
            SortOptions.ComplLapsDesc => row => -row.CompletedLaps,
            SortOptions.IncsAsc => row => row.Incidents,
            SortOptions.IncsDesc => row => -row.Incidents,
            SortOptions.LeadLapsAsc => row => row.LeadLaps,
            SortOptions.LeadLapsDesc => row => -row.LeadLaps,
            SortOptions.PenPtsAsc => row => row.PenaltyPoints,
            SortOptions.PenPtsDesc => row => -row.PenaltyPoints,
            SortOptions.PosAsc => row => row.Position,
            SortOptions.PosDesc => row => -row.Position,
            SortOptions.RacePtsAsc => row => row.RacePoints,
            SortOptions.RacePtsDesc => row => -row.RacePoints,
            SortOptions.TotalPtsAsc => row => row.TotalPoints,
            SortOptions.TotalPtsDesc => row => -row.TotalPoints,
            SortOptions.LastRaceOrderAsc => row => row.ResultRows.LastOrDefault()?.FinalPosition ?? 999,
            SortOptions.LastRaceOrderDesc => row => -(row.ResultRows.LastOrDefault()?.FinalPosition ?? 999),
            SortOptions.WinsAsc => row => row.Wins,
            SortOptions.WinsDesc => row => -row.Wins,
            SortOptions.Top3Asc => row => row.Top3,
            SortOptions.Top3Desc => row => -row.Top3,
            SortOptions.Top5Asc => row => row.Top5,
            SortOptions.Top5Desc => row => -row.Top5,
            SortOptions.Top10Asc => row => row.Top10,
            SortOptions.Top10Desc => row => -row.Top10,
            SortOptions.RacesAsc => row => row.Races,
            SortOptions.RacesDesc => row => -row.Races,
            SortOptions.RacesCountedAsc => row => row.RacesCounted,
            SortOptions.RacesCountedDesc => row => -row.RacesCounted,
            SortOptions.RacesScoredAsc => row => row.RacesScored,
            SortOptions.RacesScoredDesc => row => -row.RacesScored,
            SortOptions.RacesInPointsAsc => row => row.RacesInPoints,
            SortOptions.RacesInPointsDesc => row => -row.RacesInPoints,
            _ => row => 0,
        };
    }

    private static TimeSpan GetLapTimeSortValue(TimeSpan lapTime)
    {
        return lapTime != TimeSpan.Zero ? lapTime : TimeSpan.MaxValue;
    }
}

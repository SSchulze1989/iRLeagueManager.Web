using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Services.ResultService.Calculation;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.Tests.ResultService.Calculation;

internal static class CalculationMockHelper
{
    internal static PointRule<ResultRowCalculationResult> MockPointRule(
        FilterGroupRowFilter<ResultRowCalculationResult>? pointFilters = default,
        FilterGroupRowFilter<ResultRowCalculationResult>? resultFilters = default,
        FilterGroupRowFilter<ResultRowCalculationResult>? champSeasonFilters = default,
        Func<IEnumerable<ResultRowCalculationResult>, IReadOnlyList<ResultRowCalculationResult>>? sortForPoints = default,
        Func<IEnumerable<ResultRowCalculationResult>, IReadOnlyList<ResultRowCalculationResult>>? sortFinal = default,
        Func<ResultRowCalculationResult, int, double>? getRacePoints = default,
        IEnumerable<BonusPointConfiguration>? bonusPoints = default,
        IEnumerable<AutoPenaltyConfigurationData>? autoPenalties = default)
    {
        pointFilters ??= new(Array.Empty<(FilterCombination, RowFilter<ResultRowCalculationResult>)>());
        resultFilters ??= new(Array.Empty<(FilterCombination, RowFilter<ResultRowCalculationResult>)>());
        champSeasonFilters ??= new(Array.Empty<(FilterCombination, RowFilter<ResultRowCalculationResult>)>());
        sortForPoints ??= row => row.ToList();
        sortFinal ??= row => row.ToList();
        getRacePoints ??= (row, pos) => row.RacePoints;
        bonusPoints ??= Array.Empty<BonusPointConfiguration>();
        autoPenalties ??= Array.Empty<AutoPenaltyConfigurationData>();
        return MockPointRule<ResultRowCalculationResult>(
            pointFilters,
            resultFilters,
            champSeasonFilters,
            sortForPoints,
            sortFinal,
            getRacePoints,
            bonusPoints,
            autoPenalties);
    }

    internal static PointRule<T> MockPointRule<T>(
        FilterGroupRowFilter<T> pointFilters,
        FilterGroupRowFilter<T> finalFilters,
        FilterGroupRowFilter<T> champSeasonFilters,
        Func<IEnumerable<T>, IReadOnlyList<T>> sortForPoints,
        Func<IEnumerable<T>, IReadOnlyList<T>> sortFinal,
        Func<T, int, double> getRacePoints,
        IEnumerable<BonusPointConfiguration> bonusPoints,
        IEnumerable<AutoPenaltyConfigurationData> autoPenalties) where T : IPointRow, IPenaltyRow
    {
        var mockRule = new Mock<PointRule<T>>();
        mockRule.Setup(x => x.GetPointFilters()).Returns(pointFilters);
        mockRule.Setup(x => x.GetResultFilters()).Returns(finalFilters);
        mockRule.Setup(x => x.GetChampSeasonFilters()).Returns(champSeasonFilters);
        mockRule
            .Setup(x => x.SortForPoints(It.IsAny<IEnumerable<T>>()))
            .Returns((IEnumerable<T> rows) => sortForPoints(rows).ToList());
        mockRule
            .Setup(x => x.SortFinal(It.IsAny<IEnumerable<T>>()))
            .Returns((IEnumerable<T> rows) => sortFinal(rows).ToList());
        mockRule
            .Setup(x => x.ApplyPoints(It.IsAny<SessionCalculationData>(), It.IsAny<IReadOnlyList<T>>()))
            .Returns((SessionCalculationData _, IEnumerable<T> rows) =>
            {
                foreach ((T row, int pos) in rows.Select((x, i) => (x, i + 1)))
                {
                    row.RacePoints = getRacePoints(row, pos);
                }
                return rows.ToList();
            });
        mockRule.Setup(x => x.GetBonusPoints()).Returns(bonusPoints);
        mockRule.Setup(x => x.GetAutoPenalties()).Returns(autoPenalties);
        return mockRule.Object;
    }
}

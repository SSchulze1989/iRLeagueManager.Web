using iRLeagueApiCore.Common.Models;
using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;

internal abstract class PointRule<TRow> where TRow : IPointRow, IPenaltyRow
{
    public abstract FilterGroupRowFilter<TRow> GetPointFilters();
    public abstract FilterGroupRowFilter<TRow> GetResultFilters();
    public abstract FilterGroupRowFilter<TRow> GetChampSeasonFilters();
    public abstract IEnumerable<AutoPenaltyConfigurationData> GetAutoPenalties();
    public abstract IEnumerable<BonusPointConfiguration> GetBonusPoints();
    public abstract IReadOnlyList<T> SortForPoints<T>(IEnumerable<T> rows) where T : TRow;
    public abstract IReadOnlyList<T> ApplyPoints<T>(SessionCalculationData session, IReadOnlyList<T> rows) where T : TRow;
    public abstract IReadOnlyList<T> SortFinal<T>(IEnumerable<T> rows) where T : TRow;

    private readonly static DefaultPointRule<TRow> defaultPointRule = new();
    public static PointRule<TRow> Default() => defaultPointRule;
}

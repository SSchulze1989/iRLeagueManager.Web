using iRLeagueApiCore.Common.Enums;
using iRLeagueApiCore.Services.ResultService.Models;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace iRLeagueApiCore.Services.ResultService.Calculation;
internal sealed class IdRowFilter<TId> : RowFilter<ResultRowCalculationResult>
{
    public IReadOnlyCollection<TId?> MatchIds;
    public Func<ResultRowCalculationResult, TId?> GetIdFunc;
    public MatchedValueAction Action;

    /// <summary>
    /// Create new instance of member or team row filter
    /// </summary>
    /// <param name="idValues"></param>
    /// <param name="action"></param>
    /// <exception cref="ArgumentException">If id cannot be parsed as long</exception>
    public IdRowFilter(IEnumerable<string> idValues, Func<ResultRowCalculationResult, TId?> getIdFunc, MatchedValueAction action)
    {
        MatchIds = idValues.Select(GetIdValue).ToList();
        GetIdFunc = getIdFunc;
        Action = action;
    }

    public override IEnumerable<T> FilterRows<T>(IEnumerable<T> rows)
    {
        var match = rows.Where(x => MatchIds.Contains(GetIdFunc(x)));
        return Action switch
        {
            MatchedValueAction.Keep => match,
            MatchedValueAction.Remove => rows.Except(match),
            _ => rows,
        };
    }

    private TId? GetIdValue(string idString)
    {
        try
        {
            return (TId?)Convert.ChangeType(idString, typeof(TId?), CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException) 
        {
            throw new ArgumentException($"Argument \"{idString}\" is not a valid id");
        }
    }
}


using iRLeagueApiCore.Services.ResultService.Models;
using NCalc;

namespace iRLeagueApiCore.Services.ResultService.Calculation;
internal class FormulaPointRule : CalculationPointRuleBase
{
    public string Formula { get; }
    public bool AllowNegativePoints { get; }
    private static IDictionary<string, FormulaParameter> _parameters = FormulaParameters.ParameterDict;

    public FormulaPointRule(string formula, bool allowNegativePoints)
    {
        Formula = formula;
        AllowNegativePoints = allowNegativePoints;
    }

    public override IReadOnlyList<T> ApplyPoints<T>(SessionCalculationData session, IReadOnlyList<T> rows)
    {
        // prepare parameters
        var e = new NCalc.Expression(Formula, EvaluateOptions.IterateParameters);
        foreach (var (key, parameter) in _parameters)
        {
            e.Parameters[key] = rows.Select(row => parameter.valueFunc.Invoke(session, row)).ToArray();
        }
        // calculate
        if (e.Evaluate() is not IList<object> points)
        {
            return rows;
        }
        // assign points to rows
        foreach (var (row, rowPoints) in rows.Zip(points))
        {
            row.RacePoints = Convert.ToDouble(rowPoints);
            if (!AllowNegativePoints)
            {
                row.RacePoints = Math.Max(row.RacePoints, 0);
            }
        }
        return rows;
    }
}

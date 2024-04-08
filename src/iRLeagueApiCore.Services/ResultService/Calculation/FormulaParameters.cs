using iRLeagueApiCore.Services.ResultService.Models;

namespace iRLeagueApiCore.Services.ResultService.Calculation;
internal record FormulaParameter(string[] Aliases, string Description, Func<SessionCalculationData, ResultRowCalculationData, double> valueFunc);

public static class FormulaParameters
{
    internal static IEnumerable<FormulaParameter> Parameters { get; } = new List<FormulaParameter>()
    {
        new(["pos", "position"], "Finish position", (_, row) => row.FinishPosition),
        new(["start", "start_position"], "Starting position", (_, row) => row.StartPosition),
        new(["irating"], "Irating at the start of the session", (_, row) => row.OldIrating),
        new(["sof", "strength_of_field"], "SOF - Strength of field (Irating)", (session, _) => session.Sof),
        new(["count", "driver_count"], "Number of drivers/teams in the result", (session, _) => session.ResultRows.Count()),
        new(["flap", "fastest_lap"], "Personal fastest lap", (_, row) => row.FastestLapTime.TotalSeconds),
        new(["qlap", "qualy_lap"], "Personal qualy lap", (_, row) => row.QualifyingTime.TotalSeconds),
        new(["avglap", "avg_lap"], "Personal avg. lap", (_, row) => row.AvgLapTime.TotalSeconds),
        new(["flapsession", "session_fastest_lap"], "Fastest lap in the session", (session, _) => session.FastestLap.TotalSeconds),
        new(["qlapsession", "session_fastest_qualy_lap"], "Fastest qualy lap in the session", (session, _) => session.FastestQualyLap.TotalSeconds),
        new(["avglapsession", "session_fastest_avg_lap"], "Fastest avg. lap in the session", (session, _) => session.FastestAvgLap.TotalSeconds),
    };

    internal static IDictionary<string, FormulaParameter> ParameterDict => Parameters
        .SelectMany(x => x.Aliases.Select(y => (name: y, parameter: x)))
        .ToDictionary(k => k.name, v => v.parameter);

    public static IEnumerable<(string[] aliases, string description)> ParameterInfo => Parameters.Select(x => (x.Aliases, x.Description));
}

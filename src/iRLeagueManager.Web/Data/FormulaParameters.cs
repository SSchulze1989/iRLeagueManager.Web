namespace iRLeagueManager.Web.Data;

public record FormulaParameter(string[] Aliases, string Description);

public static class FormulaParameters
{
    public static IEnumerable<FormulaParameter> Parameters { get; } = new List<FormulaParameter>()
    {
        new(["pos", "position"], "Finish position"),
        new(["start", "start_position"], "Starting position"),
        new(["irating"], "Irating at the start of the session"),
        new(["sof", "strength_of_field"], "SOF - Strength of field (Irating)"),
        new(["count", "driver_count"], "Number of drivers/teams in the result"),
        new(["flap", "fastest_lap"], "Personal fastest lap"),
        new(["qlap", "qualy_lap"], "Personal qualy lap"),
        new(["avglap", "avg_lap"], "Personal avg. lap"),
        new(["flapsession", "session_fastest_lap"], "Fastest lap in the session"),
        new(["qlapsession", "session_fastest_qualy_lap"], "Fastest qaly lap in the session"),
        new(["avglapsession", "session_fastest_avg_lap"], "Fastest avg. lap in the session"),
    };

    public static IDictionary<string, FormulaParameter> ParameterDict => Parameters
        .SelectMany(x => x.Aliases.Select(y => (name: y, parameter: x)))
        .ToDictionary(k => k.name, v => v.parameter);

    public static IEnumerable<(string[] aliases, string description)> ParameterInfo => Parameters.Select(x => (x.Aliases, x.Description));
}

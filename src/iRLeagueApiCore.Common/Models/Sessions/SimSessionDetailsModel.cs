namespace iRLeagueApiCore.Common.Models;

public class SimSessionDetailsModel
{
    public long SessionDetailsId { get; set; }
    public long IRSubsessionId { get; set; }
    public int IRRaceWeek { get; set; }
    public long IRSessionId { get; set; }
    public int LicenseCategory { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int EventStrengthOfField { get; set; }
    public TimeSpan EventAverageLap { get; set; }
    public int EventLapsComplete { get; set; }
    public int TimeOfDay { get; set; }
    public int WeatherType { get; set; }
    public int TempUnits { get; set; }
    public int TempValue { get; set; }
    public int RelHumidity { get; set; }
    public int Fog { get; set; }
    public int WindDir { get; set; }
    public int WindUnits { get; set; }
    public int Skies { get; set; }
    public int WeatherVarInitial { get; set; }
    public int WeatherVarOngoing { get; set; }
    public DateTime? SimStartUtcTime { get; set; }
    public TimeSpan SimStartUtcOffset { get; set; }
}

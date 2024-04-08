namespace iRLeagueApiCore.Client.ResultsParsing;

#pragma warning disable IDE1006 // Benennungsstile
public sealed class ParseWeather
{
    public int type { get; set; }
    public int temp_units { get; set; }
    public int temp_value { get; set; }
    public int rel_humidity { get; set; }
    public int fog { get; set; }
    public int wind_dir { get; set; }
    public int wind_units { get; set; }
    public int wind_value { get; set; }
    public int skies { get; set; }
    public int weather_var_initial { get; set; }
    public int weather_var_ongoing { get; set; }
    public int time_of_day { get; set; }
    public DateTime simulated_start_utc_time { get; set; }
    public long simulated_start_utc_offset { get; set; }
}

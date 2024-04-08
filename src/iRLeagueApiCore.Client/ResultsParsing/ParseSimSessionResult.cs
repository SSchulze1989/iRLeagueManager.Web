namespace iRLeagueApiCore.Client.ResultsParsing;

#pragma warning disable IDE1006 // Benennungsstile
public sealed class ParseSimSessionResult
{
    public long subsession_id { get; set; }
    public long season_id { get; set; }
    public string? season_name { get; set; }
    public string? season_short_name { get; set; }
    public int season_year { get; set; }
    public int season_quarter { get; set; }
    public int series_id { get; set; }
    public string? series_name { get; set; }
    public string? series_short_name { get; set; }
    public int race_week_num { get; set; }
    public long session_id { get; set; }
    public int license_category_id { get; set; }
    public string? license_category { get; set; }
    public long private_session_id { get; set; }
    public long host_id { get; set; }
    public string? session_name { get; set; }
    public long league_id { get; set; }
    public string? league_name { get; set; }
    public long league_season_id { get; set; }
    public string? league_season_name { get; set; }
    public bool restrict_results { get; set; }
    public DateTime start_time { get; set; }
    public DateTime end_time { get; set; }
    public int num_laps_for_qual_average { get; set; }
    public int num_laps_for_solo_average { get; set; }
    public int corners_per_lap { get; set; }
    public int caution_type { get; set; }
    public int event_type { get; set; }
    public string? event_type_name { get; set; }
    public bool driver_changes { get; set; }
    public int min_team_drivers { get; set; }
    public int max_team_drivers { get; set; }
    public int driver_change_rule { get; set; }
    public int driver_change_param1 { get; set; }
    public int driver_change_param2 { get; set; }
    public int max_weeks { get; set; }
    public string? points_type { get; set; }
    public int event_strength_of_field { get; set; }
    public int event_average_lap { get; set; }
    public int event_laps_complete { get; set; }
    public int num_cautions { get; set; }
    public int num_caution_laps { get; set; }
    public int num_lead_changes { get; set; }
    public bool official_session { get; set; }
    public int heat_info_id { get; set; }
    public int special_event_type { get; set; }
    public int damage_model { get; set; }
    public bool can_protest { get; set; }
    public int cooldown_minutes { get; set; }
    public int limit_minutes { get; set; }
    public ParseTrack track { get; set; } = new();
    public ParseWeather weather { get; set; } = new();
    public ParseTrackState track_state { get; set; } = new();
    public ParseSessionResult[] session_results { get; set; } = Array.Empty<ParseSessionResult>();
    public ParseCarClass[] car_classes { get; set; } = Array.Empty<ParseCarClass>();
}

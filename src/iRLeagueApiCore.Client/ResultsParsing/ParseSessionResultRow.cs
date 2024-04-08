namespace iRLeagueApiCore.Client.ResultsParsing;

#pragma warning disable IDE1006 // Benennungsstile
public sealed class ParseSessionResultRow
{
    public long? team_id { get; set; }
    public long cust_id { get; set; }
    public string? display_name { get; set; }
    public int finish_position { get; set; }
    public int finish_position_in_class { get; set; }
    public int laps_lead { get; set; }
    public int laps_complete { get; set; }
    public int opt_laps_complete { get; set; }
    public int interval { get; set; }
    public int class_interval { get; set; }
    public long average_lap { get; set; }
    public int best_lap_num { get; set; }
    public long best_lap_time { get; set; }
    public int best_nlaps_num { get; set; }
    public long best_nlaps_time { get; set; }
    public DateTime best_qual_lap_at { get; set; }
    public int best_qual_lap_num { get; set; }
    public long best_qual_lap_time { get; set; }
    public int reason_out_id { get; set; }
    public string? reason_out { get; set; }
    public int champ_points { get; set; }
    public bool drop_race { get; set; }
    public int club_points { get; set; }
    public int position { get; set; }
    public long qual_lap_time { get; set; }
    public int starting_position { get; set; }
    public int starting_position_in_class { get; set; }
    public int car_class_id { get; set; }
    public string? car_class_name { get; set; }
    public string? car_class_short_name { get; set; }
    public int club_id { get; set; }
    public string? club_name { get; set; }
    public string? club_shortname { get; set; }
    public int division { get; set; }
    public int old_license_level { get; set; }
    public int old_sub_level { get; set; }
    public double old_cpi { get; set; }
    public int oldi_rating { get; set; }
    public int old_ttrating { get; set; }
    public int new_license_level { get; set; }
    public int new_sub_level { get; set; }
    public double new_cpi { get; set; }
    public int newi_rating { get; set; }
    public int new_ttrating { get; set; }
    public int multiplier { get; set; }
    public int license_change_oval { get; set; }
    public int license_change_road { get; set; }
    public int incidents { get; set; }
    public int max_pct_fuel_fill { get; set; }
    public int weight_penalty_kg { get; set; }
    public int league_points { get; set; }
    public int league_agg_points { get; set; }
    public int car_id { get; set; }
    public string? car_name { get; set; }
    public int aggregate_champ_points { get; set; }
    public ParseLivery livery { get; set; } = new();
    public bool watched { get; set; }
    public bool friend { get; set; }
    public ParseSessionResultRow[] driver_results { get; set; } = Array.Empty<ParseSessionResultRow>();
    public bool ai { get; set; }
}

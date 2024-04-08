namespace iRLeagueApiCore.TrackImport.Models;

public struct TrackImportModel
{
    public bool ai_enabled { get; set; }
    public bool award_exempt { get; set; }
    public string? category { get; set; }
    public int category_id { get; set; }
    public DateTime closes { get; set; }
    public string? config_name { get; set; }
    public int corners_per_lap { get; set; }
    public DateTime created { get; set; }
    public bool free_with_subscription { get; set; }
    public bool fully_lit { get; set; }
    public int grid_stalls { get; set; }
    public bool has_opt_path { get; set; }
    public bool has_short_parade_lap { get; set; }
    public bool has_start_zone { get; set; }
    public bool has_svg_map { get; set; }
    public bool is_dirt { get; set; }
    public bool is_oval { get; set; }
    public int lap_scoring { get; set; }
    public double latitude { get; set; }
    public string? location { get; set; }
    public double longitude { get; set; }
    public int max_cars { get; set; }
    public bool night_lighting { get; set; }
    public double nominal_lap_time { get; set; }
    public int number_pitstalls { get; set; }
    public string? opens { get; set; }
    public int package_id { get; set; }
    public int pit_road_speed_limit { get; set; }
    public double price { get; set; }
    public int priority { get; set; }
    public bool purchasable { get; set; }
    public int qualify_laps { get; set; }
    public bool restart_on_left { get; set; }
    public bool retired { get; set; }
    public string? search_filters { get; set; }
    public string? site_url { get; set; }
    public int sku { get; set; }
    public int solo_laps { get; set; }
    public bool start_on_left { get; set; }
    public bool supports_grip_compound { get; set; }
    public bool tech_track { get; set; }
    public string? time_zone { get; set; }
    public double track_config_length { get; set; }
    public string? track_dirpath { get; set; }
    public int track_id { get; set; }
    public string? track_name { get; set; }

    public TrackTypeImportModel[]? track_types { get; set; }
}

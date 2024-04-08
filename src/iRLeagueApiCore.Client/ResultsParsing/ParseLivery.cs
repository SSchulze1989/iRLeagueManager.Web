namespace iRLeagueApiCore.Client.ResultsParsing;

#pragma warning disable IDE1006 // Benennungsstile
public sealed class ParseLivery
{
    public int car_id { get; set; }
    public int pattern { get; set; }
    public string? color1 { get; set; }
    public string? color2 { get; set; }
    public string? color3 { get; set; }
    public int number_font { get; set; }
    public string? number_color1 { get; set; }
    public string? number_color2 { get; set; }
    public string? number_color3 { get; set; }
    public int number_slant { get; set; }
    public int sponsor1 { get; set; }
    public int sponsor2 { get; set; }
    public string? car_number { get; set; }
    public string? wheel_color { get; set; }
    public int rim_type { get; set; }
}

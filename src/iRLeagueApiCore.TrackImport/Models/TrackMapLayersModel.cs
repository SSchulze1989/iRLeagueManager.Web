using System.Text.Json.Serialization;

namespace iRLeagueApiCore.TrackImport.Models;

public struct TrackMapLayersModel
{
    public string background { get; set; }
    public string inactive { get; set; }
    public string active { get; set; }
    public string pitroad { get; set; }
    [JsonPropertyName("start-finish")]
    public string start_finish { get; set; }
    public string turns { get; set; }
}

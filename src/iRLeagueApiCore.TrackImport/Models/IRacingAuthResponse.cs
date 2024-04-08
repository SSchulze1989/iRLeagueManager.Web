using iRLeagueApiCore.TrackImport.Converters;
using System.Text.Json.Serialization;

namespace iRLeagueApiCore.TrackImport.Models;

public sealed class IRacingAuthResponse
{
    [JsonPropertyName("authcode"), JsonConverter(typeof(JsonStringOrNumberToStringConverter))]
    public string? Authcode { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace iRLeagueApiCore.TrackImport.Converters;

public sealed class JsonStringOrNumberToStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.GetInt32().ToString(),
            _ => null,
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

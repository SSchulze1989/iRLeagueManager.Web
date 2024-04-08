using System.Text.Json;

namespace iRLeagueApiCore.Common.Converters;

public sealed class JsonTimeSpanToTicksConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeof(long).IsAssignableFrom(typeToConvert))
        {
            var longValue = reader.GetInt64();
            return new TimeSpan(longValue);
        }
        return TimeSpan.Zero;
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Ticks);
    }
}

#if NETCOREAPP
using System.Text.Json;

namespace iRLeagueApiCore.Common.Converters;

public class JsonTimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeof(long).IsAssignableFrom(typeToConvert))
        {
            var longValue = reader.GetInt64();
            return new TimeSpan(longValue);
        }
        else
        {
            var timeSpanString = reader.GetString();
            if (string.IsNullOrWhiteSpace(timeSpanString))
            {
                return TimeSpan.Zero;
            }
            var hasDays = timeSpanString.Count(c => c == '.') > 1;
            if (hasDays)
            {
                return TimeSpan.ParseExact(timeSpanString, @"dd\.hh\:mm\:ss\.fffff", null);
            }
            return TimeSpan.ParseExact(timeSpanString, @"hh\:mm\:ss\.fffff", null);
        }
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        if (value.Days > 0)
        {
            writer.WriteStringValue(value.ToString(@"dd\.hh\:mm\:ss\.fffff"));
            return;
        }
        writer.WriteStringValue(value.ToString(@"hh\:mm\:ss\.fffff"));
    }
}
#endif

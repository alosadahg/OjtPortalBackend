using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OjtPortal.Infrastructure.JsonConverters
{
    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private string[] formats;
        public TimeOnlyConverter()
        {
            formats = new[]
            {
                "HH:mm:ss", // 24-hr format with seconds (22:50:30)
                "HH:mm", // 24-hr format (22:50)
                "hh:mm tt" // 12-hr format with AM/PM (10:50 PM)
            };
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var timeString = reader.GetString();
                foreach (var format in formats)
                {
                    if (TimeOnly.TryParseExact(timeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                    {
                        return time;
                    }
                }
                throw new JsonException($"Unable to convert {timeString} to {nameof(TimeOnly)} using the supported formats. Example accepted time formats are: [22:00:00, 01:00, 01:00 PM]");
            }
            throw new JsonException("Expected a string token for TimeOnly.");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            var timeString = value.ToString("hh:ss tt");
            writer.WriteStringValue(timeString);
        }
    }
}

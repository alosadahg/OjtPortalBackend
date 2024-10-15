using System.Text.Json;
using System.Text.Json.Serialization;

namespace OjtPortal.Infrastructure.JsonConverters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var manilaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(value, manilaTimeZone);
            string formattedDate = localTime.ToString("ddd MMM dd yyyy HH:mm:ss zz\\:ff", System.Globalization.CultureInfo.InvariantCulture);
            writer.WriteStringValue(formattedDate);
        }
    }
}

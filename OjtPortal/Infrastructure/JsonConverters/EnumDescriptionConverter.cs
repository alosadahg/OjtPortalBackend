using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OjtPortal.Infrastructure.JsonConverters
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class EnumDescriptionConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var enumType = typeof(T);
                var description = reader.GetString();
    
                Console.WriteLine($"Received value: {description}");

                foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                    if (attribute != null && attribute.Description == description)
                    {
                        return (T)field.GetValue(null)!;
                    }
                }
                throw new JsonException($"Unable to convert \"{description}\" to enum type {enumType}.");
            }
            throw new JsonException("Expected a string token.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var enumType = typeof(T);
            var field = enumType.GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            var description = attribute?.Description ?? value.ToString();
            writer.WriteStringValue(description);
        }
    }

}

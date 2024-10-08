using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<SentimentCategory>))]
    public enum SentimentCategory
    {
        [Description("Negative")]
        Negative,
        [Description("Positive")]
        Positive,
        [Description("Neutral")]
        Neutral
    }
}

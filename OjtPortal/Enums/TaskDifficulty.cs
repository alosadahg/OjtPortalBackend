using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<TaskDifficulty>))]
    public enum TaskDifficulty
    {
        [Description("Easy")]
        Easy,
        [Description("Medium")]
        Medium,
        [Description("Hard")]
        Hard
    }
}

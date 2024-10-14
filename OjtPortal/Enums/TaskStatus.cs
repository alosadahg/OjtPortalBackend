using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<TaskStatus>))]
    public enum TaskStatus
    {
        [Description("Not Started")]
        NotStarted,
        [Description("In Progress")]
        InProgress,
        [Description("Done")]
        Done
    }
}

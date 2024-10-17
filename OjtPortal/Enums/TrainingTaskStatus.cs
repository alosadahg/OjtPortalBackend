using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<TrainingTaskStatus>))]
    public enum TrainingTaskStatus
    {
        [Description("Not Started")]
        NotStarted,
        [Description("In Progress")]
        InProgress,
        [Description("Done")]
        Done
    }
}

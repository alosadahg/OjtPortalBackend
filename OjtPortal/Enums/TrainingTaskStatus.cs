using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<TrainingTaskStatus>))]
    public enum TrainingTaskStatus
    {
        [Description("NotStarted")]
        NotStarted,
        [Description("InProgress")]
        InProgress,
        [Description("Done")]
        Done,
        [Description("DoneLate")]
        DoneLate
    }
}

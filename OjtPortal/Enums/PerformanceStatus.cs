using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<PerformanceStatus>))]
    public enum PerformanceStatus
    {
        [Description("On Track")]
        OnTrack,
        [Description("Off Track")]
        OffCourse
    }
}

using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(PerformanceStatus))]
    public enum PerformanceStatus
    {
        [Description("On Track")]
        OnTrack,
        [Description("Off Track")]
        OffCourse
    }
}

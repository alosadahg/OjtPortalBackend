using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<WorkingDays>))]
    public enum WorkingDays
    {
        [Description("WeekdaysOnly")]
        WeekdaysOnly,
        [Description("WeekdaysAndSaturdays")]
        WeekdaysAndSaturdays,
        [Description("WholeWeek")]
        WholeWeek
    }
}

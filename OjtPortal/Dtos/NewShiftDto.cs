using OjtPortal.Enums;
using OjtPortal.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class NewShiftDto
    {
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly? Start { get; set; } = null;
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly? End { get; set; } = null;
        public int DailyDutyHrs { get; set; } = 0;
        public WorkingDays WorkingDays { get; set; } = WorkingDays.WeekdaysOnly;
    }
}

using OjtPortal.Enums;
using OjtPortal.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class NewShiftDto
    {
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly Start { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly End { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
        public int DailyDutyHrs { get; set; } = 0;
        public WorkingDays WorkingDays { get; set; } = WorkingDays.WeekdaysOnly;
    }
}

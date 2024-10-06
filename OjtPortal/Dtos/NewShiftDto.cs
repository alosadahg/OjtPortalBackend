using OjtPortal.Enums;
using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class NewShiftDto
    {
        [Required(ErrorMessage = "Shift start time is required")]
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly? Start { get; set; } = null;
        [Required(ErrorMessage = "Shift end time is required")]
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly? End { get; set; } = null;
        [Required(ErrorMessage = "Daily duty hours is required")]
        public int DailyDutyHrs { get; set; } = 0;
        [Required(ErrorMessage = "Working days is required")]
        public WorkingDays WorkingDays { get; set; } = WorkingDays.WeekdaysOnly;
    }
}

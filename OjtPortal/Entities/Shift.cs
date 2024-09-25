using OjtPortal.Enums;
using OjtPortal.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class Shift
    {
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly Start { get; set; } = new();
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly End { get; set; } = new();
        public int DailyDutyHrs { get; set; } = 0;
        public int LateTimeInCount { get; set; } = 0;
        public WorkingDays WorkingDays { get; set; } = WorkingDays.WeekdaysOnly;
        public int AbsencesCount { get; set; } = 0;
        public double TotalHrsRendered { get; set; } = 0;

        public Shift()
        {
        }

        public Shift(TimeOnly shiftStart, TimeOnly shiftEnd, int dailyDutyHrs, int lateTimeInCount, int absencesCount, WorkingDays workingDays)
        {
            WorkingDays = workingDays;
            Start = shiftStart;
            End = shiftEnd;
            DailyDutyHrs = dailyDutyHrs;
            LateTimeInCount = lateTimeInCount;
            AbsencesCount = absencesCount;
        }
    }
}

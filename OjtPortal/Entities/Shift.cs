using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Enums;
using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class Shift
    {
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly? Start { get; set; }
        [JsonConverter(typeof(TimeOnlyConverter))]
        public TimeOnly? End { get; set; }
        public int DailyDutyHrs { get; set; } = 0;
        public int LateTimeInCount { get; set; } = 0;
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<WorkingDays>))]
        public WorkingDays WorkingDays { get; set; } = WorkingDays.WeekdaysOnly;
        public int AbsencesCount { get; set; } = 0;
        public double TotalHrsRendered { get; set; } = 0;
        public int TotalManDaysRendered { get; set; } = 0;
        public bool IncludePublicPhHolidays { get; set; } = false;

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

        public Shift(TimeOnly? start, TimeOnly? end, int dailyDutyHrs, int lateTimeInCount, WorkingDays workingDays, int absencesCount, double totalHrsRendered, bool includePublicPhHolidays)
        {
            Start = start;
            End = end;
            DailyDutyHrs = dailyDutyHrs;
            LateTimeInCount = lateTimeInCount;
            WorkingDays = workingDays;
            AbsencesCount = absencesCount;
            TotalHrsRendered = totalHrsRendered;
            IncludePublicPhHolidays = includePublicPhHolidays;
        }
    }
}

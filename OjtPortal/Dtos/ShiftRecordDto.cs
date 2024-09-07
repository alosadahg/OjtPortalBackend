using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class ShiftRecordDto
    {
        public int? StudentId { get; set; }
        public TimeOnly ShiftStart { get; set; } = new();
        public TimeOnly ShiftEnd { get; set; } = new();
        public int DailyDutyHrs { get; set; } = 0;
    }
}

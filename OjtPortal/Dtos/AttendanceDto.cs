using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class AttendanceDto
    {
        public long AttendanceId { get; set; }
        public int StudentId { get; set; }
        public DateTime TimeIn { get; set; } = DateTime.UtcNow;
        public DateTime? TimeOut { get; set; } = null;
        public bool IsTimeOutLate { get; set; }
        public bool IsTimeInLate { get; set; }
        public double RenderedHours { get; set; } = 0;
        public LogbookEntry? LogbookEntry { get; set; } = null;
    }
}

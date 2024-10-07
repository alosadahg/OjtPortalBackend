namespace OjtPortal.Entities
{
    public class Attendance
    {
        public long AttendanceId { get; set; } 
        public int StudentId {  get; set; }
        public Student? Student { get; set; } = null;
        public DateTime? TimeIn { get; set; } = null;
        public DateTime? TimeOut { get; set; } = null;
        public double RenderedHours { get; set; } = 0;
        public LogbookEntry? LogbookEntry { get; set; }

        public Attendance()
        {
        }
    }
}

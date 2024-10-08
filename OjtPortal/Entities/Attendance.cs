using OjtPortal.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class Attendance
    {
        public long AttendanceId { get; set; } 
        public int StudentId {  get; set; }
        [JsonIgnore]
        public Student Student { get; set; } = new();
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime TimeIn { get; set; } = DateTime.UtcNow;
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? TimeOut { get; set; } = null;
        public bool IsTimeOutLate {  get; set; } 
        public double RenderedHours { get; set; } = 0;
        public LogbookEntry? LogbookEntry { get; set; } = null;

        public Attendance()
        {
        }
    }
} 

using OjtPortal.Entities;
using OjtPortal.Infrastructure.JsonConverters;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class AttendanceDto
    {
        public long AttendanceId { get; set; }
        public int StudentId { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime TimeIn { get; set; } = DateTime.UtcNow;
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? TimeOut { get; set; } = null;
        public bool IsTimeOutLate { get; set; }
        public bool IsTimeInLate { get; set; }
        public double RenderedHours { get; set; } = 0;
        public LogbookEntry? LogbookEntry { get; set; } = null;
    }
}

using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Enums;
using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class LogbookEntry
    {
        [Key]
        [Column("LogbookEntryId")]
        public long AttendanceId {  get; set; }
        [JsonIgnore]
        [ForeignKey("AttendanceId")]
        public Attendance Attendance { get; set; } = new();
        public string Activities { get; set; } = string.Empty;

        public string Remarks {  get; set; } = string.Empty;
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<SentimentCategory>))]
        public SentimentCategory? RemarkSentimentCategory { get; set; } = null;
        public double? RemarkSentimentScore { get; set; } = null;
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<LogbookStatus>))]
        public LogbookStatus LogbookStatus { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? CreationTimestamp { get; set; } = null;
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? SubmissionTimestamp { get; set; } = null;
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? RemarksTimestamp { get; set; } = null;

    }
}

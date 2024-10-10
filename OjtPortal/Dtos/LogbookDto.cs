using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class LogbookDto
    {
        public Attendance? Attendance { get; set; } = null;
        public string Activities { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;
        
        public SentimentCategory? RemarkSentimentCategory { get; set; } = null;
        public double? RemarkSentimentScore { get; set; } = null;
        public LogbookStatus LogbookStatus { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? CreationTimestamp { get; set; } = null;
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? SubmissionTimestamp { get; set; } = null;
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? RemarksTimestamp { get; set; } = null;
    }
}

using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime? CreationTimestamp { get; set; } = null;
        public DateTime? SubmissionTimestamp { get; set; } = null;
        public DateTime? RemarksTimestamp { get; set; } = null;
    }
}

﻿using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Entities
{
    public class LogbookEntry
    {
        [Key]
        [Column("LogbookEntryId")]
        public long AttendanceId {  get; set; }
        [ForeignKey("AttendanceId")]
        public Attendance? Attendance { get; set; } = null;
        public string Activities { get; set; } = string.Empty;

        public string Remarks {  get; set; } = string.Empty;
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<LogbookStatus>))]
        public LogbookStatus LogbookStatus { get; set; }
        public DateTime? CreationTimestamp { get; set; } = null;
        public DateTime? SubmissionTimestamp { get; set; } = null;
        public DateTime? RemarksTimestamp { get; set; } = null;

    }
}
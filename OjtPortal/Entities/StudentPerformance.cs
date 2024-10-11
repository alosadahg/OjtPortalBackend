using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Entities
{
    public class StudentPerformance
    {
        [Key]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<PerformanceStatus>))]
        public PerformanceStatus PerformanceStatus { get; set; }
        public string LogbookStatusRemarks { get; set; } = string.Empty;
        public int AttendanceCount { get; set; } = 0;
        public int LogbookCount { get; set; } = 0;
        public double RemainingHoursToRender { get; set; } = 0;
        public int RemainingManDays { get; set; } = 0;

    }
}

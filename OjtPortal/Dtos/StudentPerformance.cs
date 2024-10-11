using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Dtos
{
    public class StudentPerformance
    {
        public StudentPerformance()
        {
        }

        public ExistingUserDto? User { get; set; } = null;
        public string StudentId { get; set; } = string.Empty;
        public DegreeProgramDto? DegreeProgram { get; set; } = null;
        public string Designation { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; } = null;
        public DateOnly? EndDate { get; set; } = null;
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
        public Shift? Shift { get; set; } = null;
        public PerformanceStatus PerformanceStatus { get; set; }
        public string StatusRemarks { get; set; } = string.Empty;
        public int AttendanceCount { get; set; } = 0;
        public int LogbookCount { get; set; } = 0;
        public double RemainingHoursToRender { get; set; } = 0;
        public int RemainingManDays { get; set; } = 0;


    }
}

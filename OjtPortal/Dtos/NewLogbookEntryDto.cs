using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Dtos
{
    public class NewLogbookEntryDto
    {
        [Required(ErrorMessage ="Attendance id must not be null")]
        public long AttendanceId { get; set; }
        [Required(ErrorMessage = "Activities must not be null")]
        public string Activities { get; set; } = string.Empty;
    }
}

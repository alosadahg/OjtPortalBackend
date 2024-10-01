using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OjtPortal.Entities
{
    public class Student 
    {
        [Key]
        [Column("Id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string StudentId { get; set; } = "";
        public DegreeProgram? DegreeProgram { get; set; } = new();
        public int? DegreeProgramId { get; set; } = 0;
        public int? MentorId { get; set; }
        public Mentor? Mentor { get; set; }
        public int? InstructorId { get; set; }
        public Teacher? Instructor { get; set; }
        public string Division { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        [Column(TypeName = "varchar(50)")]
        [ValueConverter(typeof(EnumToStringConverter<InternshipStatus>))]
        public InternshipStatus InternshipStatus { get; set; }
        public Shift? Shift { get; set; } = new();

        public Student()
        {
        }

    }
}

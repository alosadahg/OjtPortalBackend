using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public DegreeProgram DegreeProgram { get; set; } = new();
        public int DegreeProgramId { get; set; } = 0;
        public int MentorId { get; set; }
        public Mentor Mentor { get; set; } = new();
        public int InstructorId { get; set; }
        public Teacher Instructor { get; set; } = new();
        public string Division { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new DateOnly();
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; }
        public Shift Shift { get; set; } = new();

        public Student()
        {
        }

    }
}

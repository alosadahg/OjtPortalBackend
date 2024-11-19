using OjtPortal.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class StudentDto
    {
        public ExistingUserDto? User { get; set; } = null;
        public string StudentId { get; set; } = string.Empty;
        public DegreeProgramDto? DegreeProgram { get; set; } = null;
        public string Designation { get; set; } = string.Empty;
        public MentorDto? Mentor { get; set; } = null;
        public int? InstructorId { get; set; } = null;
        public string Division { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; } = null;
        public DateOnly? EndDate { get; set; } = null;
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
        public Shift? Shift { get; set; } = null;

        public StudentDto()
        {
        }
    }

    public class NewStudentDto : NewUserDto
    {
        public string StudentId { get; set; } = string.Empty;
        public int? DegreeProgramId { get; set; } = null;
        public string? Designation { get; set; } = string.Empty;
        public int? MentorId { get; set; } = null;
        public int? TeacherId { get; set; } = null;
        public string? Division { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; } = null;
        public int HrsToRender { get; set; } = 0;
        public NewShiftDto? Shift { get; set; } = null;
    }

    public class MentorAddStudentDto : UserDto
    {
        [Required(ErrorMessage = "Intern designation is required")]
        public string Designation { get; set; } = string.Empty;
        [Required(ErrorMessage = "Mentor ID is required")]
        public int MentorId { get; set; } = 0;
        [Required(ErrorMessage = "Assigned division for intern is required")]
        public string Division { get; set; } = string.Empty;
        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required(ErrorMessage = "Hours to render is required")]
        public int HrsToRender { get; set; } = 0;
        [Required(ErrorMessage = "Shift information is required")]
        public NewShiftDto Shift { get; set; } = new();
    }

    public class TeacherAddStudentDto : UserDto
    {
        [Required(ErrorMessage = "Instructor ID is required")]
        public int InstructorId { get; set; }
        [Required(ErrorMessage = "Student ID is required")]
        public string StudentId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Degree program ID is required")]
        public int? DegreeProgramId { get; set; } = null;
    }

    public class UpdateStudentDto
    {
        public ExistingUserDto? User { get; set; } = null;
        public string StudentId { get; set; } = string.Empty;
        public DegreeProgram? DegreeProgram { get; set; } = null;
        public int? InstructorId { get; set; } = null;
    }
}

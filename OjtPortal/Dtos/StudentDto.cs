using OjtPortal.Entities;

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
        public string Designation { get; set; } = string.Empty;
        public int? MentorId { get; set; } = null;
        public int? TeacherId { get; set; } = null;
        public string Division { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; } = null;
        public int HrsToRender { get; set; } = 0;
        public NewShiftDto? Shift { get; set; } = null;
    }

    public class MentorAddStudentDto : UserDto
    {

        public string Designation { get; set; } = string.Empty;
        public int MentorId { get; set; } = 0;
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int HrsToRender { get; set; } = 0;
        public NewShiftDto Shift { get; set; } = new();
    }

    public class UpdateStudentDto
    {
        public ExistingUserDto? User { get; set; } = null;
        public string StudentId { get; set; } = string.Empty;
        public DegreeProgram? DegreeProgram { get; set; } = null;
        public int? InstructorId { get; set; } = null;
    }
}

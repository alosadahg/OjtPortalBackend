using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class StudentDto
    {
        public FullUserDto User { get; set; } = new();
        public string StudentId { get; set; } = string.Empty;
        public DegreeProgramDto DegreeProgram { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public int MentorId { get; set; } = 0;
        public int InstructorId { get; set; } = 0;
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new();
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
        public Shift Shift { get; set; } = new();

        public StudentDto()
        {
        }
    }

    public class NewStudentDto : NewUserDto
    {
        public string StudentId { get; set; } = string.Empty;
        public int DegreeProgramId { get; set; } = 0;
        public string Designation { get; set; } = string.Empty;
        public int? MentorId { get; set; } = null;
        public int? TeacherId { get; set; } = null;
        public string Division { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; } = null;
        public int HrsToRender { get; set; } = 0;
        public NewShiftDto Shift { get; set; } = new();
    }
}

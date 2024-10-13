using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class StudentToMentorOverviewDto 
    {
        public ExistingUserDto User { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public TeacherNoStudentsDto? Instructor { get; set; }
        public DateOnly? StartDate { get; set; } = null;
        public DateOnly? EndDate { get; set; } = null;
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
        public Shift Shift { get; set; } = new();
    }

    public class StudentToInstructorOverviewDto 
    {
        public ExistingUserDto User { get; set; } = new();
        public string StudentId { get; set; } = string.Empty;
        public DegreeProgram DegreeProgram { get; set; } = new();
        public MentorDto? Mentor { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; } = null;
        public DateOnly? EndDate { get; set; } = null;
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
        public Shift? Shift { get; set; } = null;
    }
}

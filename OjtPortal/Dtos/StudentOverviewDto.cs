using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class StudentToMentorOverviewDto : UserDto
    {
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new();
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
    }

    public class StudentToInstructorOverviewDto : UserDto
    {
        public string StudentId { get; set; } = string.Empty;
        public DegreeProgram DegreeProgram { get; set; } = new();
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public MentorDto Mentor { get; set; } = new();
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new();
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
        public ShiftRecordDto ShiftRecord { get; set; } = new();
    }
}

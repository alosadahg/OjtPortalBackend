using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class StudentDto : UserDto
    {
        public string StudentId { get; set; } = string.Empty;
        public int DegreeProgramId { get; set; } = 0;
        public int MentorId { get; set; } = 0;
        public int InstructorId { get; set; } = 0;
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new DateOnly();
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; }
        public ShiftRecordDto ShiftRecord { get; set; } = new();

        public StudentDto()
        {
        }
    }
}

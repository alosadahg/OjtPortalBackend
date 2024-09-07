using OjtPortal.Entities;
using OjtPortal.Enums;

namespace OjtPortal.Dtos
{
    public class NewStudentDto : NewUserDto
    {
        public string StudentId { get; set; } = string.Empty;
        public int DegreeProgramId { get; set; } = 0;
        public int MentorId { get; set; } = 0;
        public int InstructorId { get; set; } = 1;
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int HrsToRender { get; set; } = 0;
        public int ShiftRecordId { get; set; } = 0;
    }
}

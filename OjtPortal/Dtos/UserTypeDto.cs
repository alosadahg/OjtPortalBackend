namespace OjtPortal.Dtos
{
    public class UserTypeDto
    {
        public ExistingUserDto? Admin { get; set; }
        public TeacherDto? Teacher { get; set; }
        public StudentDto? Student { get; set; }
        public MentorDto? Mentor { get; set; }
    }
}

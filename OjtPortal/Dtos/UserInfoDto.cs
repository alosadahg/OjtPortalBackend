namespace OjtPortal.Dtos
{
    public class UserInfoDto
    {
        public ExistingUserDto? Admin { get; set; }
        public ChairDto? Chair { get; set; }
        public TeacherDto? Teacher { get; set; }
        public StudentDto? Student { get; set; }
        public FullMentorDto? Mentor { get; set; }
        public UserInfoDto() { }    

    }
}

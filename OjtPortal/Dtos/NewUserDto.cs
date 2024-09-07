namespace OjtPortal.Dtos
{

    public class NewUserDto : UserDto
    {
        public string Password { get; set; } = string.Empty;
        public NewUserDto()
        {
        }

    }
}

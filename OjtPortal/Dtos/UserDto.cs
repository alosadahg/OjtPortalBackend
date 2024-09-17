using OjtPortal.Enums;

namespace OjtPortal.Dtos
{
    public class UserDto { 
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class ExistingUserDto : UserDto
    {
        public int Id { get; set; } = 0;
    }

    public class NewUserDto : UserDto
    {
        public string Password { get; set; } = string.Empty;
        public NewUserDto()
        {
        }

    }
}

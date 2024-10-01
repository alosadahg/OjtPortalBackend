using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class UserDto {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class ExistingUserDto : UserDto
    {
        public int Id { get; set; } = 0;
        public UserType UserType { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }

    public class NewUserDto : UserDto
    {
        public string? Password { get; set; } = null;
        public NewUserDto()
        {
        }

    }

    public class FullUserDto : UserDto
    {
        public int Id { get; set; }
        public UserType UserType { get; set; }
    }
}

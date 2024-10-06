using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class UserDto {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public UserDto(string email, string firstName, string lastName)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public UserDto()
        {
        }
    }

    public class ExistingUserDto : UserDto
    {
        public int Id { get; set; } = 0;
        public UserType UserType { get; set; }
        public AccountStatus AccountStatus { get; set; }

        public ExistingUserDto(int id, UserType userType, AccountStatus accountStatus, string email, string firstName, string lastName) : base(email, firstName, lastName)
        {
            Id = id;
            UserType = userType;
            AccountStatus = accountStatus;
        }

        public ExistingUserDto()
        {
        }
    }

    public class NewUserDto : UserDto
    {
        public string? Password { get; set; } = null;
        public NewUserDto()
        {
        }

    }
}

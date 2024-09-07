using OjtPortal.Enums;

namespace OjtPortal.Dtos
{
    public interface IUserDto
    {
        AccountStatus AccountStatus { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        UserType UserType { get; set; }
    }

    public class UserDto : IUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public AccountStatus AccountStatus { get; set; }

        public UserDto()
        {
        }
    }
}

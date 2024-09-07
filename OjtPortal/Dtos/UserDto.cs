using OjtPortal.Enums;

namespace OjtPortal.Dtos
{
    public class UserDto { 
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserDto()
        {
        }
    }
}

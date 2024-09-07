using Microsoft.AspNetCore.Identity;
using OjtPortal.Enums;

namespace OjtPortal.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserType UserType { get; set; }
        public AccountStatus AccountStatus { get; set; }

        public User() { }

        public User(string firstName, string lastName, UserType userType, AccountStatus accountStatus)
        {
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
            AccountStatus = accountStatus;
        }
        public User(User user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserType = user.UserType;
            AccountStatus = user.AccountStatus;
        }

    }
}

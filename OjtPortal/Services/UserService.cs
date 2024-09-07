using Microsoft.AspNetCore.Identity;
using OjtPortal.Entities;

namespace OjtPortal.Services
{
    public class UserService
    {
        private readonly UserManager<User> _usermanager;

        public UserService(UserManager<User> usermanager)
        {
            this._usermanager = usermanager;
        }
        
        
    }
}

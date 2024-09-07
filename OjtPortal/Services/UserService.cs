using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OjtPortal.Dtos;
using OjtPortal.Entities;

namespace OjtPortal.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(NewUserDto user);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _usermanager;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> usermanager, IMapper mapper)
        {
            this._usermanager = usermanager;
            this._mapper = mapper;
        }

        public async Task CreateUserAsync(NewUserDto user)
        {
            var userEntity = _mapper.Map<User>(user);
            userEntity.UserName = user.Email;
            var result = await _usermanager.CreateAsync(userEntity, user.Password);

            if (result.Succeeded)
            {
                Console.WriteLine("User created successfully.");
                // TODO: Add more logic if needed
            }
            else
            {
                Console.WriteLine("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

    }
}

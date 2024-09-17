using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using System.Security.Cryptography;
using System.Text;

namespace OjtPortal.Services
{
    public interface IUserService
    {
        Task<(User?, ErrorResponseModel?)> CreateUserAsync(NewUserDto newUser, UserType userType);
        string GeneratePassword();
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _usermanager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UserService(UserManager<User> usermanager, IMapper mapper, ILogger<UserService> logger)
        {
            this._usermanager = usermanager;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<(User?, ErrorResponseModel?)> CreateUserAsync(NewUserDto newUser, UserType userType)
        {
            var userEntity = _mapper.Map<User>(newUser);
            userEntity.UserName = newUser.Email;
            userEntity.UserType = userType;

            var result = await _usermanager.CreateAsync(userEntity, newUser.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully.");
            }
            else
            {
                _logger.LogError("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                return (null, ErrorHandler.GetIdentityErrorResponse(result.Errors, newUser.Email));
            }

            return (userEntity, null);
        }

        public string GeneratePassword()
        {
            var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789!@#$%^&*()/";

            StringBuilder result = new StringBuilder(8);

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[1];
                for (int i = 0; i < 8; i++)
                {
                    rng.GetBytes(randomNumber);
                    int randomIndex = randomNumber[0] % characters.Length;

                    result.Append(characters[randomIndex]);
                }
            }

            return result.ToString();
        }


        /*public Task<ErrorResponseModel?> SendActivationEmailAsync(string emailTo, User user, string password)
        {

        }*/
    }
}

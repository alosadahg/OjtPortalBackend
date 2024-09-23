using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace OjtPortal.Services
{
    public interface IUserService
    {
        Task<(User?, ErrorResponseModel?)> CreateUserAsync(NewUserDto newUser, UserType userType);
        Task<(string?, ErrorResponseModel?)> ActivateAccountAsync(int userId, string code);
        Task<ErrorResponseModel?> SendActivationEmailAsync(string emailTo, User user, string password);
        Task<string?> GetActivationLinkAsync(User user);
        string GeneratePassword();
    }

    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IUserRepo _userRepository;
        private readonly LinkGenerator _linkGenerator;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<User> _signInManager;

        public UserService(IMapper mapper, ILogger<UserService> logger, IUserRepo userRepository, LinkGenerator linkGenerator, IEmailSender emailSender, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager)
        {
            this._mapper = mapper;
            this._logger = logger;
            this._userRepository = userRepository;
            this._linkGenerator = linkGenerator;
            this._emailSender = emailSender;
            this._userManager = userManager;
            this._httpContextAccessor = httpContextAccessor;
            this._signInManager = signInManager;
        }

        public async Task<(User?, ErrorResponseModel?)> CreateUserAsync(NewUserDto newUser, UserType userType)
        {
            var userEntity = _mapper.Map<User>(newUser);
            userEntity.UserName = newUser.Email;
            userEntity.UserType = userType;

            if (string.IsNullOrEmpty(newUser.Password)) newUser.Password = GeneratePassword();

            var (result, error) = await _userRepository.CreateAsync(userEntity, newUser.Password);

            if (error != null)
            {
                _logger.LogError("User creation failed: " + newUser.Email);
                return (null, error);
            }

            return (result, null);
        }

        public async Task<(User?, ErrorResponseModel?)> GetUserByIdAsync(int id)
        {
            var (result, error) = await _userRepository.GetUserByIdAsync(id);

            if (error != null) return (null, error);

            return (result, null);
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

        public async Task<ErrorResponseModel?> SendActivationEmailAsync(string emailTo, User user, string password)
        {
            var activationLink = await GetActivationLinkAsync(user);
            try
            {
                await _emailSender.SendEmailAsync(emailTo, "Activate your account", EmailTemplate.ActivationEmailTemplate(user.Email!, password, activationLink!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new(HttpStatusCode.RequestTimeout, "Failed to Send Email", "Unable to send after 3 attempts");
            }
            return null;
        }

        public async Task<(string?, ErrorResponseModel?)> ActivateAccountAsync(int userId, string code)
        {
            var (user, error) = await _userRepository.GetUserByIdAsync(userId);
            if(error == null) return (null, error);

            await _userManager.ConfirmEmailAsync(user!, code);
            user!.AccountStatus  = AccountStatus.Active;
            return ("Thank you for activating your account", null);
        }
        public async Task<string?> GetActivationLinkAsync(User user)
        { 
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var userId = await _userManager.GetUserIdAsync(user);
            string confirmationLink = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext!, "ActivateAccount", "User", new { userId = userId, token = token })!;
            return confirmationLink!;
        }

        /*public async Task<(ExistingUserDto?, ErrorResponseModel?)> LoginAsync(LoginDto loginDto)
        {
            _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

            var (user, error) = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if(error != null) return (null, error);

            if(user!.AccountStatus == AccountStatus.Pending)
                return (null, new(HttpStatusCode.UnprocessableContent, new ErrorModel("Inactive account", "Activate the account first using the activation email")));
            if (user!.AccountStatus == AccountStatus.Deactivated)
                return (null, new(HttpStatusCode.UnprocessableContent, new ErrorModel("Deactivated account", "Access privileges are revoked for this account")));
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, true, false);
            if (!result.Succeeded)
                return (null, new(HttpStatusCode.Unauthorized, "Login failed", "Invalid credentials"));

            
        }*/
    }
}

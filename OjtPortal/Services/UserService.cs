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
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace OjtPortal.Services
{
    public interface IUserService
    {
        Task<(CreatedUserDto?, ErrorResponseModel?)> CreateUserAsync(UserDto newUser, string password, UserType userType);
        Task<(string?, ErrorResponseModel?)> ActivateAccountAsync(int userId, string code);
        Task<ErrorResponseModel?> SendActivationEmailAsync(string emailTo, User user, string password);
        Task<ErrorResponseModel?> SendActivationEmailAsync(string emailTo, User user);
        Task<(string?, ErrorResponseModel?)> ResendActivationEmailAsync(string emailTo);
        Task<(SignInResult?, ErrorResponseModel?)> LoginAsync(LoginDto loginDto);
        string GenerateToken(string type);
        Task<(string?, ErrorResponseModel?)> ForgetPasswordAsync(string email);
        Task<(string?, ErrorResponseModel?)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<(string?, ErrorResponseModel?)> ChangeDefaultPasswordAsync(ChangeDefaultPasswordDto changePasswordDto, string token, bool isActive);
        Task<(ExistingUserDto?, ErrorResponseModel?)> PermanentlyRemoveUserAsync(int id);
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
        private readonly IOtpRepo _otpRepo;

        public UserService(IMapper mapper, ILogger<UserService> logger, IUserRepo userRepository, LinkGenerator linkGenerator, IEmailSender emailSender, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager, IOtpRepo otpRepo)
        {
            this._mapper = mapper;
            this._logger = logger;
            this._userRepository = userRepository;
            this._linkGenerator = linkGenerator;
            this._emailSender = emailSender;
            this._userManager = userManager;
            this._httpContextAccessor = httpContextAccessor;
            this._signInManager = signInManager;
            this._otpRepo = otpRepo;
        }

        public async Task<(CreatedUserDto?, ErrorResponseModel?)> CreateUserAsync(UserDto newUser, string password, UserType userType)
        {
            var userEntity = _mapper.Map<User>(newUser);
            userEntity.UserName = newUser.Email;
            userEntity.UserType = userType;
            var response = new CreatedUserDto();

            if (string.IsNullOrEmpty(password))
            {
                password = GenerateToken("password");
                response.IsPasswordGenerated = true;
                response.Password = password;
                userEntity.AccountStatus = AccountStatus.PendingPasswordChange;
            }

            var (result, error) = await _userRepository.CreateAsync(userEntity, password);

            if (error != null)
            {
                _logger.LogError("User creation failed: " + newUser.Email);
                return (null, error);
            }

            response.User = result;
            response.Password = password;
            return (response, null);
        }

        public async Task<(User?, ErrorResponseModel?)> GetUserByIdAsync(int id)
        {
            var (result, error) = await _userRepository.GetUserByIdAsync(id);

            if (error != null) return (null, error);

            return (result, null);
        }


        public string GenerateToken(string type)
        {
            var otp = "1234567890";
            var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789!@()[]";
            var source = string.Empty;

            var length = 0;
            if(type.ToLower().Equals("otp"))
            {
                length = 6;
                source = otp;
            } 
            else if (type.ToLower().Equals("password"))
            {
                length = 15;
                source = characters;
            }
            StringBuilder result = new StringBuilder(length);

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[1];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(randomNumber);
                    int randomIndex = randomNumber[0] % source.Length;

                    result.Append(source[randomIndex]);
                }
            }

            return result.ToString();
        }

        public async Task<(string?, ErrorResponseModel?)> ResendActivationEmailAsync(string emailTo)
        {
            var (user, error) = await _userRepository.GetUserByEmailAsync(emailTo);
            if (error != null) return (null, error);
            var emailError = await SendActivationEmailAsync(emailTo, user!);
            if (emailError != null) return (null, emailError);
            return ("Activation email successfully sent. Please check inbox.", null);
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

        public async Task<ErrorResponseModel?> SendActivationEmailAsync(string emailTo, User user)
        {
            var activationLink = await GetActivationLinkAsync(user);
            try
            {
                await _emailSender.SendEmailAsync(emailTo, "Activate your account", EmailTemplate.ActivationEmailTemplate(activationLink!));
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
            if(error != null) return (null, error);

            if(user!.AccountStatus == AccountStatus.Deactivated)
            {
                return (null, new(HttpStatusCode.UnprocessableContent, "Deactivated Account", "Access privileges are revoked for this account"));
            } 
            if(user!.AccountStatus == AccountStatus.Active)
            {
                return ("Account is already activated", null);
            }
            if(user!.AccountStatus == AccountStatus.PendingPasswordChange)
            {
                var changePasswordUrl = _linkGenerator.GetPathByPage("/ChangeDefaultPassword", null,
                    new
                    {
                        Id = user.Id,
                        Token = code,
                        PendingStudentUpdate = (user.UserType.Equals(UserType.Student)) ? true : false,
                        PendingEmailUpdate = (user.UserType.Equals(UserType.Student)) && !EmailChecker.IsEmailValid(user.Email!)
                    });
                return (changePasswordUrl, null);
            }
            user = await _userRepository.ActivateAccount(user, code);
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

        public async Task<(SignInResult?, ErrorResponseModel?)> LoginAsync(LoginDto loginDto)
        {
            _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

            var (user, error) = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (error != null) return (null, error);

            if (user!.AccountStatus == AccountStatus.Pending || user!.AccountStatus == AccountStatus.PendingPasswordChange)
                return (null, new(HttpStatusCode.UnprocessableContent, new ErrorModel("Inactive account", "Activate the account first using the activation email")));
            if (user!.AccountStatus == AccountStatus.Deactivated)
                return (null, new(HttpStatusCode.MethodNotAllowed, new ErrorModel("Deactivated account", "Access privileges are revoked for this account")));
            var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if(checkPassword.Succeeded) return (await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, true, false), null);
            return (null, new(HttpStatusCode.BadRequest, "Login failed", "Invalid credentials"));
        }

        public async Task<(string?, ErrorResponseModel?)> ForgetPasswordAsync(string email)
        {
            var (user, error) = await _userRepository.GetUserByEmailAsync(email);
            if (error != null) return (null, error);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user!);
            var otpCode = GenerateToken("otp");
            OTP? otp = new OTP
            {
                UserId = user!.Id,
                User = user!,
                Code = otpCode,
                Token = token
            };
            otp = await _otpRepo.AddOTPAsync(otp);
            if (otp == null) return (null, new(HttpStatusCode.BadRequest, "Forget Password failed", "Failed to generate otp"));
            await _emailSender.SendEmailAsync(email, "Forget Password OTP", EmailTemplate.OTPTemplate(otp!.Code));
            return ("OTP is sent to email", null);
        }

        public async Task<(string?, ErrorResponseModel?)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var (user, error) = await _userRepository.GetUserByEmailAsync(resetPasswordDto.Email);
            if (error != null) return (null, error);
            var otp = await _otpRepo.GetOTPByIdAsync(user!.Id);
            if(otp == null) return (null, new(HttpStatusCode.NotFound, "Failed to reset password", "No forget password request"));
            if (!otp.Code.Equals(resetPasswordDto.Code)) return (null, new(HttpStatusCode.BadRequest, "Invalid OTP", "OTP is either expired or invalid"));
            await _userManager.ResetPasswordAsync(user, otp.Token, resetPasswordDto.NewPassword);
            await _otpRepo.RemoveOTP(otp);
            return ("Successfully changed password", null);
        }

        public async Task<(string?, ErrorResponseModel?)> ChangeDefaultPasswordAsync(ChangeDefaultPasswordDto changePasswordDto, string token, bool isActive)
        {
			var (user, error) = await _userRepository.GetUserByIdAsync(changePasswordDto.Id);
            if (error != null) return (null, error);

            if (user!.AccountStatus == AccountStatus.Active)
            {
                return (null, new(HttpStatusCode.BadRequest, "Active Account", "Account is already activated. New password is not saved. Use forget password instead to reset existing password."));
            }
            if (user!.AccountStatus == AccountStatus.Deactivated)
            {
                return (null, new(HttpStatusCode.BadRequest, "Deactivated Account", "Access Privileges are revoked for this account"));
            }

            if (!changePasswordDto.NewPassword.Equals(changePasswordDto.ConfirmPassword))
            {
                return (null, new(HttpStatusCode.BadRequest, "Password mismatch", "Re-enter password and try again"));
            }

            if (isActive)
            {
                user = await _userRepository.ActivateAccount(user, token);
            } 

            var code = await _userManager.GeneratePasswordResetTokenAsync(user!);
            await _userManager.ResetPasswordAsync(user!, code, changePasswordDto.NewPassword);
            return ("Successfully changed password", null);
        }

        public async Task<(ExistingUserDto?, ErrorResponseModel?)> PermanentlyRemoveUserAsync(int id)
        {
            var deletedUser = await _userRepository.DeleteByIdAsync(id);
            if (deletedUser == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("user"), LoggingTemplate.MissingRecordDescription("user", id.ToString())));
            return (_mapper.Map<ExistingUserDto>(deletedUser), null);
        }

       /* public async Task<(ExistingUserDto?, ErrorResponseModel?)> ChangeEmailAsync(string existingEmail, string newEmail)
        {
            var (user, error) = await _userRepository.GetUserByEmailAsync(existingEmail);
            if (error != null) return (null, error);
            var (existingUser, _) = await _userRepository.GetUserByEmailAsync(newEmail);
            if (existingUser != null) return (null, new(HttpStatusCode.BadRequest, "Email unavailable", "Cannot use this email"));
            if(!user.AccountStatus.Equals(AccountStatus.Active)) return (null, new(HttpStatusCode.BadRequest, "Account not active", "Activate account first"));
        }*/
    }
}

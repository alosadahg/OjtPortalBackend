using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using System.Net;
using System.Text;

namespace OjtPortal.Repositories
{
    public interface IUserRepo
    {
        Task<(User?, ErrorResponseModel?)> CreateAsync(User user, string password);
        Task<(User?, ErrorResponseModel?)> GetUserByIdAsync(int id);
        Task<(User?, ErrorResponseModel?)> GetUserByEmailAsync(string email);
        Task<User> ActivateAccount(User user, string token);
        Task<User?> DeleteByIdAsync(int id);
    }

    public class UserRepo : IUserRepo
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserRepo> _logger;
        private readonly OjtPortalContext _context;

        public UserRepo(UserManager<User> userManager, ILogger<UserRepo> logger, OjtPortalContext context)
        {
            this._userManager = userManager;
            this._logger = logger;
            this._context = context;
        }

        public async Task<(User?, ErrorResponseModel?)> CreateAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully.");
                return (user, null);
            }
            _logger.LogError("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            return (null, ErrorHandler.GetIdentityErrorResponse(result.Errors, user.Email));
        }

        public async Task<(User?, ErrorResponseModel?)> GetUserByIdAsync(int id)
        {
            var result = await _context.Users.FindAsync(id);

            if (result == null)
            {
                _logger.LogError($"User not found for Id: {id}");
                return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle("user"), LoggingTemplate.MissingRecordDescription("user", id.ToString()))));
            }
            return (result, null);
        }

        public async Task<(User?, ErrorResponseModel?)> GetUserByEmailAsync(string email)
        {
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (result != null)
            {
                return (result, null);
            }
            _logger.LogError($"User not found for email: {email}");
            return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle("user"), LoggingTemplate.MissingRecordDescription("user", email))));
        }

        public async Task<User> ActivateAccount(User user, string token)
        {
            user.AccountStatus = AccountStatus.Active;
            _context.Entry(user).State = EntityState.Modified;
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            await _userManager.ConfirmEmailAsync(user, token);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteByIdAsync(int id)
        {
            var (user, _) = await GetUserByIdAsync(id);
            if (user == null) return null;
            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();
            return user;
        }
/*
        public async Task<User?> ChangeEmailAsync (User user, string newEmail)
        {
            var (user, _) = await GetUserByIdAsync(id);
            if (user == null) return null;
            _context.Users.Remove(user!);
            await _context.SaveChangesAsync();
            return user;
        }*/
    }
}

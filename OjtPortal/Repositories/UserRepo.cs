using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using System.Net;

namespace OjtPortal.Repositories
{
    public interface IUserRepo
    {
        Task<(User?, ErrorResponseModel?)> CreateAsync(User user, string password);
        Task<(User?, ErrorResponseModel?)> GetUserByIdAsync(int id);
        Task<(User?, ErrorResponseModel?)> GetUserByEmailAsync(string email);
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
                return (result, null);
            }
            _logger.LogError($"User not found for id: {id}");
            return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle("user"), LoggingTemplate.MissingRecordDescription("user", id.ToString()))));
        }

        public async Task<(User?, ErrorResponseModel?)> GetUserByEmailAsync(string email)
        {
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (result == null)
            {
                return (result, null);
            }
            _logger.LogError($"User not found for email: {email}");
            return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle("user"), LoggingTemplate.MissingRecordDescription("user", email))));
        }

    }
}

using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IAdminService
    {
        Task<(ExistingUserDto?, ErrorResponseModel?)> CreateAdminAsync(UserDto newUser, string password, UserType userType);
        Task<(ExistingUserDto?, ErrorResponseModel?)> GetAdminByIdAsync(int id);
    }

    public class AdminService : IAdminService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IAdminRepo _adminRepo;

        public AdminService(IUserService userService, IMapper mapper, IAdminRepo adminRepo)
        {
            this._userService = userService;
            this._mapper = mapper;
            this._adminRepo = adminRepo;
        }
        public async Task<(ExistingUserDto?, ErrorResponseModel?)> CreateAdminAsync(UserDto newUser, string password, UserType userType)
        {
            var (user, error) = await _userService.CreateUserAsync(newUser, password, userType);
            if (error != null) return (null, error);


            if (user!.IsPasswordGenerated)
            {
                password = user.Password;
                var emailError = _userService.SendActivationEmailAsync(newUser.Email, user.User!, password);
                if (emailError.Result != null) return (null, emailError.Result);
            }
            else
            {
                var emailError = _userService.SendActivationEmailAsync(newUser.Email, user.User!);
                if (emailError.Result != null) return (null, emailError.Result);
            }
            return (_mapper.Map<ExistingUserDto>(user.User), null);
        }

        public async Task<(ExistingUserDto?, ErrorResponseModel?)> GetAdminByIdAsync(int id)
        {
            var existingAdmin = await _adminRepo.GetAdminByIdAsync(id);
            if (existingAdmin == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("admin"), LoggingTemplate.MissingRecordDescription("admin", $"{id}")));
            var adminDto = _mapper.Map<ExistingUserDto>(existingAdmin);
            return (adminDto, null);
        }
    }
}

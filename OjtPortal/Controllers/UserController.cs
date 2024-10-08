using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Services;
using System.Net;
using System.Net.Http.Headers;

namespace OjtPortal.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : OjtPortalBaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IAdminService _adminService;
        private readonly IChairService _chairService;
        private readonly ITeacherService _teacherService;
        private readonly IMentorService _mentorService;
        private readonly IStudentService _studentService;

        public UserController(IUserService userService, UserManager<User> userManager, IAdminService adminService, IChairService chairService, ITeacherService teacherService, IMentorService mentorService, IStudentService studentService)
        {
            this._userService = userService;
            this._userManager = userManager;
            this._adminService = adminService;
            this._chairService = chairService;
            this._teacherService = teacherService;
            this._mentorService = mentorService;
            this._studentService = studentService;
        }

        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var (result, error) = await _userService.LoginAsync(loginDto);
            if (error != null) return MakeErrorResponse(error);
            return Empty;
        }

        /// <summary>
        /// Return logged in user information
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                switch (user.UserType)
                {
                    case UserType.Student:
                        var (student, error) = await _studentService.GetStudentByIdAsync(user.Id, true, true, true);
                        if (error != null) return MakeErrorResponse(error);
                        return Ok(student);
                    case UserType.Admin:
                        var (admin, aError) = await _adminService.GetAdminByIdAsync(user.Id);
                        if (aError != null) return MakeErrorResponse(aError);
                        return Ok(admin);
                    case UserType.Teacher:
                        var (teacher, tError) = await _teacherService.GetTeacherByIdAsync(user.Id, true);
                        if (tError != null) return MakeErrorResponse(tError);
                        return Ok(teacher);
                    case UserType.Mentor:
                        var (mentor, mError) = await _mentorService.GetMentorByIdAsync(user.Id, true);
                        if (mError != null) return MakeErrorResponse(mError);
                        return Ok(mentor);
                    case UserType.Chair:
                        var (chair, cError) = await _chairService.GetChairByIdAsync(user.Id, true);
                        if (cError != null) return MakeErrorResponse(cError);
                        return Ok(chair);
                }
            }
            return MakeErrorResponse(new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("user"), LoggingTemplate.MissingRecordDescription("user", "authorization token")));
        }

        /// <summary>
        /// Activate user account
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("activate/account")]

        public async Task<IActionResult> ActivateAccount(int userId, string token)
        {
            var (result, error) = await _userService.ActivateAccountAsync(userId, token);
            if (error != null) return MakeErrorResponse(error);
			if (!string.IsNullOrEmpty(result) && result.StartsWith("/ChangeDefaultPassword"))
			{
				return Redirect(result);
			}
			return Ok(result);
        }

        /// <summary>
        /// Resend activation email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("resend/activation")]

        public async Task<IActionResult> ResendActivationEmail(string email)
        {
            var (result, error) = await _userService.ResendActivationEmailAsync(email);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Send forget password otp to email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPatch("forget/password")]

        public async Task<IActionResult> ForgetPasswordAsync(string email)
        {
            var (result, error) = await _userService.ForgetPasswordAsync(email);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Reset password with otp
        /// </summary>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPatch("reset/password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var (result, error) = await _userService.ResetPasswordAsync(resetPasswordDto);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Permanently deletes user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExistingUserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpDelete("permanent/delete/{userId}")]
        public async Task<IActionResult> PermanentlyDeleteUserAsync(int userId)
        {
            var (result, error) = await _userService.PermanentlyRemoveUserAsync(userId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

    }
}

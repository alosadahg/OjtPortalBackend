using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : OjtPortalBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }


        /// <summary>
        /// Activate user account
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("activate/account")]

        public async Task<IActionResult> ActivateAccount(int userId, string token)
        {
            var (result, error) = await _userService.ActivateAccountAsync(userId, token);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

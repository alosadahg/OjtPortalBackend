using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Enums;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/admins")]
    public class AdminController: OjtPortalBaseController
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            this._userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> AddNewAdmin(NewUserDto newUser)
        {
            var (result, error) = await _userService.CreateUserAsync(newUser, UserType.Admin);
            if(error != null)  return MakeErrorResponse(error);
            return Ok();
        }
    }
}

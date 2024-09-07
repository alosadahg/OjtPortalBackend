using Microsoft.AspNetCore.Mvc;
using OjtPortal.Dtos;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddNewUser(NewUserDto newUser)
        {
            await _userService.CreateUserAsync(newUser);
            return Ok();
        }
    }
}

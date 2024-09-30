using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/admins")]
    public class AdminController: OjtPortalBaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            this._adminService = adminService;
        }

        /// <summary>
        /// Adds a new admin
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExistingUserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost]
        public async Task<IActionResult> AddNewAdmin(UserDto newUser)
        {
            var (result, error) = await _adminService.CreateAdminAsync(newUser);
            if(error != null)  return MakeErrorResponse(error);
            return CreatedAtRoute("GetAdminById", new {id = result!.Id}, result);
        }

        /// <summary>
        /// Get admin by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExistingUserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{Id}", Name = "GetAdminById")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var (result, error) = await _adminService.GetAdminByIdAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

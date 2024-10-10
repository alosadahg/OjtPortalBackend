using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Services;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : OjtPortalBaseController
    {
        private readonly IAttendanceService _attendanceService;
        private readonly UserManager<User> _userManager;

        public AttendanceController(IAttendanceService attendanceService, UserManager<User> userManager)
        {
            this._attendanceService = attendanceService;
            this._userManager = userManager;
        }

        /// <summary>
        /// Records the time in
        /// </summary>
        /// <param name="proceedTimeIn"> Set this to true if you wish to time in on a non-working day </param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Attendance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [Authorize]
        [HttpPost("time/in")]
        public async Task<IActionResult> TimeInAsync(bool proceedTimeIn = false)
        {
            var user = await _userManager.GetUserAsync(User);
            var (result, error) = await _attendanceService.TimeInAsync(user!.Id, proceedTimeIn);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetAttendanceByIdAsync", new {id = result!.AttendanceId}, result);
        }

        /// <summary>
        /// Gets attendance by id
        /// </summary>
        /// <param name="id"> The unique identifier of the attendance </param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Attendance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [Authorize]
        [HttpGet("{id}", Name = "GetAttendanceByIdAsync")]
        public async Task<IActionResult> GetAttendanceByIdAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var (result, error) = await _attendanceService.GetAttendanceById(user!.Id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Records the time out
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Attendance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [Authorize]
        [HttpPatch("time/out")]
        public async Task<IActionResult> TimeOutAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var (result, error) = await _attendanceService.TimeOutAsync(user!.Id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

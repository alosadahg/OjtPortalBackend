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

        public AttendanceController(IAttendanceService attendanceService)
        {
            this._attendanceService = attendanceService;
        }

        /// <summary>
        /// Records the time in
        /// </summary>
        /// <param name="id"></param>
        /// <param name="proceedTimeIn"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Attendance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost("time/in")]
        public async Task<IActionResult> TimeInAsync([Required]int id, bool proceedTimeIn = false)
        {
            var (result, error) = await _attendanceService.TimeInAsync(id, proceedTimeIn);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetAttendanceByIdAsync", new {id = result!.AttendanceId}, result);
        }

        /// <summary>
        /// Gets attendance by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Attendance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}", Name = "GetAttendanceByIdAsync")]
        public async Task<IActionResult> GetAttendanceByIdAsync(int id)
        {
            var (result, error) = await _attendanceService.GetAttendanceById(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Records the time out
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Attendance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPatch("time/out")]
        public async Task<IActionResult> TimeOutAsync(int id)
        {
            var (result, error) = await _attendanceService.TimeOutAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

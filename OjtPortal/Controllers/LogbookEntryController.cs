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
    [Route("api/logbook")]
    public class LogbookDtoController : OjtPortalBaseController
    {
        private readonly ILogbookEntryService _logbookEntryService;
        private readonly UserManager<User> _userManager;

        public LogbookDtoController(ILogbookEntryService logbookEntryService, UserManager<User> userManager)
        {
            this._logbookEntryService = logbookEntryService;
            this._userManager = userManager;
        }

        /// <summary>
        /// Add a new logbook entry
        /// </summary>
        /// <param name="logbookEntryDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LogbookDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddLogbookAsync(NewLogbookEntryDto logbookEntryDto)
        {
            var user = await _userManager.GetUserAsync(User);
            var (result, error) = await _logbookEntryService.AddLogbookEntry(logbookEntryDto, user!.Id);
            if(error != null)  return MakeErrorResponse(error);
            return CreatedAtRoute("GetLogbookByIdAsync", new { id = result!.Attendance!.AttendanceId }, result);
        }

        /// <summary>
        /// Get the logbook by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LogbookDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}", Name = "GetLogbookByIdAsync")]
        public async Task<IActionResult> GetLogbookByIdAsync([Required] long id)
        {
            var (result, error) = await _logbookEntryService.GetLogbookByIdAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Add remarks to a logbook entry (marks the status from pending to submitted)
        /// </summary>
        /// <param name="id"> The unique identifier of the logbook </param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LogbookDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPatch("{id}/remarks")]
        public async Task<IActionResult> AddLogbookRemarksAsync([Required] long id, [FromBody] string remarks)
        {
            var user = await _userManager.GetUserAsync(User);
            var (result, error) = await _logbookEntryService.AddRemarksAsync(id, user!.Id, remarks);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : OjtPortalBaseController
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            this._studentService = studentService;
        }

        /// <summary>
        /// Gets the estimated end date with holiday considerations
        /// </summary>
        /// <param name="startDate">The start date of the internship. </param>
        /// <param name="includeHolidays">Include Holidays as working days. </param>
        /// <param name="workingDays">Choose the type of working days. </param>
        /// <param name="manDays">The number of working days (man-days) required to complete the OJT.</param>
        /// <returns></returns>
        [HttpGet("ojt/end-date")]
        [ProducesResponseType(typeof(DateOnly), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEndDate(int manDays, string startDate = "2024-01-01", bool includeHolidays = false, WorkingDays workingDays = WorkingDays.WeekdaysOnly)
        {
            var startDateOnly = DateOnly.Parse(startDate);
            var (response, error) = await _studentService.GetEndDate(startDateOnly, manDays, includeHolidays, workingDays);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newStudent"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent(NewStudentDto newStudent)
        {
            _studentService.RegisterStudent(newStudent);
            return Ok();
        }
    }
}

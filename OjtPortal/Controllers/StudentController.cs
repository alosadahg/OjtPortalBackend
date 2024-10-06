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
            var (response, error) = await _studentService.GetEndDateAsync(startDateOnly, manDays, includeHolidays, workingDays);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }

        /// <summary>
        /// Register new student
        /// </summary>
        /// <param name="newStudent"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost]
        public async Task<IActionResult> RegisterStudent(NewStudentDto newStudent)
        {
            var (result, error) = await _studentService.RegisterStudentAsync(newStudent);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetStudentById", new { id = result!.User!.Id }, result);
        }

        /// <summary>
        /// Gets student by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}", Name = "GetStudentById")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var (result, error) = await _studentService.GetStudentByIdAsync(id, true);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

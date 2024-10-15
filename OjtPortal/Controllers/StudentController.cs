using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Services;
using System.ComponentModel.DataAnnotations;

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
        /// <param name="requiredHours">The number of hours required to complete the OJT.</param>
        /// <param name="dailyDutyHours">The number of hours required to complete daily the OJT.</param>
        /// <returns></returns>
        /// <returns></returns>
        [HttpGet("ojt/end-date")]
        [ProducesResponseType(typeof(DateOnly), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEndDate(int requiredHours, int dailyDutyHours, string startDate = "2024-01-01", bool includeHolidays = false, WorkingDays workingDays = WorkingDays.WeekdaysOnly)
        {
            var startDateOnly = DateOnly.Parse(startDate);
            var manDays = _studentService.CalculateManDays(requiredHours, dailyDutyHours);
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
            var (result, error) = await _studentService.GetStudentByIdAsync(id, true, true, false);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
        
        /// <summary>
        /// Get the student performance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentPerformance))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}/performance")]
        public async Task<IActionResult> GetStudentPerformanceAsync ([Required] int id)
        {
            var (result, error) = await _studentService.GetStudentPerformanceAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Get all students with filtering
        /// </summary>
        /// <param name="programCode">Degree program code (ex: BSCS, BSCPE)</param>
        /// <param name="companyName">The name of the company</param>
        /// <param name="instructorId">The unique identifier of the instructor</param>
        /// <param name="designation">Designation of the intern</param>
        /// <param name="startDate">Start date of internship (format: yyyy-mm-dd)</param>
        /// <param name="endDate">End date of internship (format: yyyy-mm-dd)</param>
        /// <param name="hrsToRender">Required number of hours</param>
        /// <param name="internshipStatus">Status of the internship</param>
        /// <param name="departmentCode">Department code (ex: CCS, CEA)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<StudentDto>> GetStudentsWithFiltering(string? programCode, string? companyName, int? instructorId, string? designation, DateOnly? startDate, DateOnly? endDate, int? hrsToRender, InternshipStatus? internshipStatus, string? departmentCode)
        {
            return await _studentService.GetStudentWithFilteringAsync(companyName,programCode, instructorId, designation, startDate, endDate, hrsToRender, internshipStatus, departmentCode);
        }
    }
}

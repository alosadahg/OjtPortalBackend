using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/teachers")]
    public class TeacherController : OjtPortalBaseController
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            this._teacherService = teacherService;
        }

        /// <summary>
        /// Add new teacher
        /// </summary>
        /// <param name="newTeacherDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeacherDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost]
        public async Task<IActionResult> RegisterTeacherAsync(NewTeacherDto newTeacherDto)
        {
            var (newTeacher, error) = await _teacherService.AddNewTeacherAsync(newTeacherDto);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetTeacherById", new { id = newTeacher!.User.Id }, newTeacher);
        }

        /// <summary>
        /// Get teacher by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeacherDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}", Name = "GetTeacherById")]
        public async Task<IActionResult> GetTeacherByIdAsync(int id)
        {
            var (response, error) = await _teacherService.GetTeacherByIdAsync(id, true);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }

        /// <summary>
        /// Get students by teacher id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeacherDtoWithStudents))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}/students")]
        public async Task<IActionResult> GetStudentsByTeacherAsync(int id)
        {
            var (response, error) = await _teacherService.GetStudentsByTeacherAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }

        /// <summary>
        /// Get teachers by department code
        /// </summary>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        [HttpGet("departments/{departmentCode}")]
        public async Task<List<TeacherDto>?> GetTeachersByDepartmentAsync(DepartmentCode departmentCode)
        {
            return await _teacherService.FindTeachersByDepartmentCode(departmentCode);
        }

        /// <summary>
        /// Get teachers by department id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TeacherDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("departments/id/{departmentId}")]
        public async Task<IActionResult> GetTeachersByDepartmentIdAsync(int departmentId)
        {
            var (teachers, error) = await _teacherService.GetTeacherByDepartmentIdAsync(departmentId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(teachers);
        }

        /// <summary>
        /// Add student to teacher
        /// </summary>
        /// <param name="newStudentDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPut("student")]
        public async Task<IActionResult> AddStudentToMentorAsync(TeacherAddStudentDto newStudentDto)
        {
            var (student, error) = await _teacherService.TeacherAddStudentAsync(newStudentDto);
            if (error != null) return MakeErrorResponse(error);
            return Ok(student);
        }
    }
}

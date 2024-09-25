using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/teacher")]
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
        [HttpGet("{id}", Name = "GetTeacherById")]
        public async Task<IActionResult> GetteacherByIdAsync(int id)
        {
            var (response, error) = await _teacherService.GetTeacherByIdAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }
    }
}

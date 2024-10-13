using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Infrastructure;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/mentors")]
    public class MentorController : OjtPortalBaseController
    {
        private readonly IMentorService _mentorService;

        public MentorController(IMentorService mentorService)
        {
            this._mentorService = mentorService;
        }

        /// <summary>
        /// Register new mentor
        /// </summary>
        /// <param name="newMentorDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FullMentorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost]
        public async Task<IActionResult> RegisterMentorAsync(NewMentorDto newMentorDto)
        {
            var (newMentor, error) = await _mentorService.AddMentorAsync(newMentorDto);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetMentorById", new { id = newMentor!.User.Id}, newMentor);
        }

        /// <summary>
        /// Get mentor by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FullMentorDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}", Name = "GetMentorById")]
        public async Task<IActionResult> GetMentorByIdAsync(int id)
        {
            var (response, error) = await _mentorService.GetMentorByIdAsync(id, true);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }

        /// <summary>
        /// Get students by mentor id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FullMentorDtoWithStudents))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}/students")]
        public async Task<IActionResult> GetStudentsByMentorAsync(int id)
        {
            var (response, error) = await _mentorService.GetStudentsByMentor(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }

        /// <summary>
        /// Add student to mentor
        /// </summary>
        /// <param name="newStudentDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPut("student")]
        public async Task<IActionResult> AddStudentToMentorAsync(MentorAddStudentDto newStudentDto)
        {
            var (student, error) = await _mentorService.MentorAddStudentAsync(newStudentDto);
            if (error != null) return MakeErrorResponse(error);
            return Ok(student);
        }
    }
}

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
    }
}

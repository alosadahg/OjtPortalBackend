using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
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
        [HttpPost("register")]
        public async Task<IActionResult> RegisterMentorAsync(NewMentorDto newMentorDto)
        {
            var (newMentor, error) = await _mentorService.AddMentorAsync(newMentorDto);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetMentorById", new { id = newMentor!.User.Id}, newMentor);
        }

        [HttpGet("{id}", Name = "GetMentorById")]
        public async Task<IActionResult> GetMentorByIdAsync(int id)
        {
            var (response, error) = await _mentorService.GetMentorByIdAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }
    }
}

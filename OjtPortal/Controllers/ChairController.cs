using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Infrastructure;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/chairs")]
    public class ChairController : OjtPortalBaseController
    {
        private readonly IChairService _chairService;

        public ChairController(IChairService chairService)
        {
            this._chairService = chairService;
        }

        /// <summary>
        /// Register chair
        /// </summary>
        /// <param name="newChairDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChairDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPost]
        public async Task<IActionResult> AddChairAsync(NewTeacherDto newChairDto)
        {
            var (newChair, error) = await _chairService.AddNewChairAsync(newChairDto);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetChairById", new { id = newChair!.User.Id }, newChair);
        }

        /// <summary>
        /// Get chair by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChairDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{Id}", Name = "GetChairById")]
        public async Task<IActionResult> GetChairByIdAsync(int id)
        {
            var (response, error) = await _chairService.GetChairByIdAsync(id, true);
            if (error != null) return MakeErrorResponse(error);
            return Ok(response);
        }
    }
}

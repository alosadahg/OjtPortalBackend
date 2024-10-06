using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Infrastructure;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/degree-programs")]
    public class DegreeProgramController : OjtPortalBaseController
    {
        private readonly IDegreeProgramService _degreeProgramService;

        public DegreeProgramController(IDegreeProgramService degreeProgramService)
        {
            this._degreeProgramService = degreeProgramService;
        }

        /// <summary>
        /// Gets all degree programs available
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DegreeProgramDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet]
        public async Task<IActionResult> GetDegreeProgramsAsync()
        {
            var (degreePrograms, error) = await _degreeProgramService.GetDegreeProgramsAsync();
            if (error != null) return MakeErrorResponse(error);
            return Ok(degreePrograms);
        }

        /// <summary>
        /// Get degree programs by department id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DegreeProgramDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("department/{departmentId}")]
        public async Task<IActionResult> GetDegreeProgramsByDepartmentIdAsync(int departmentId)
        {
            var (degreePrograms, error) = await _degreeProgramService.GetDegreeProgramsByDepartmentIdAsync(departmentId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(degreePrograms);
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
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
        [HttpGet]
        public async Task<IActionResult> GetDegreeProgramsAsync()
        {
            var (degreePrograms, error) = await _degreeProgramService.GetDegreeProgramsAsync();
            if (error != null) return MakeErrorResponse(error);
            return Ok(degreePrograms);
        }
    }
}
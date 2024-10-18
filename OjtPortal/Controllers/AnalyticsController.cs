using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : OjtPortalBaseController
    {
        private readonly ISkillService _skillService;
        private readonly ITechStackService _techStackService;

        public AnalyticsController(ISkillService skillService, ITechStackService techStackService)
        {
            this._skillService = skillService;
            this._techStackService = techStackService;
        }


        /// <summary>
        /// Gets overall skill frequency in all training plans
        /// </summary>
        /// <returns></returns>
        [HttpGet("skill/frequency")]
        public async Task<List<KeyFrequency>> GetOverallSKillFrequency()
        {
            return await _skillService.GetSkillFrequencyAsync();
        }

        /// <summary>
        /// Gets the frequency by techstack name in all training plans
        /// </summary>
        /// <returns></returns>
        [HttpGet("techstack/name/frequency")]
        public async Task<List<KeyFrequency>> GetOverallTechStackNameFrequency()
        {
            return await _techStackService.GetTechStackFrequencyAsync();
        }

        /// <summary>
        /// Gets the frequency by techstack type in all training plans
        /// </summary>
        [HttpGet("techstack/type/frequency")]
        public async Task<List<KeyFrequency>> GetOverallTechStackTypeFrequency()
        {
            return await _techStackService.GetTechStackTypeFrequencyAsync();
        }
    }
}

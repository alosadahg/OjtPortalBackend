using Microsoft.AspNetCore.Mvc;
using OjtPortal.Entities;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/skills")]
    public class SkillController
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            this._skillService = skillService;
        }

        /// <summary>
        /// Gets AI generated skills with filtering
        /// </summary>
        /// <param name="nameFilter">Filters skills by name</param>
        /// <param name="descriptionFilter">Filters skills by description</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter)
        {
            return await _skillService.GetSkillsWithFilteringAsync(nameFilter, descriptionFilter);
        }

        /// <summary>
        /// Gets unique skill names
        /// </summary>
        /// <returns></returns>
        [HttpGet("unique")]
        public async Task<List<string>> GetUniqueNameSkillsAsync()
        {
            return await _skillService.GetUniqueTechStackNames();
        }
    }
}

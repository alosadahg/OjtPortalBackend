using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/techstacks")]
    public class TechStackController : OjtPortalBaseController
    {
        private readonly ITechStackService _techStackService;

        public TechStackController(ITechStackService techStackService)
        {
            this._techStackService = techStackService;
        }

        /// <summary>
        /// Get AI generated tech stacks with filtering
        /// </summary>
        /// <param name="nameFilter">Filters tech stacks by name</param>
        /// <param name="typeFilter">Filters tech stacks by type</param>
        /// <param name="descriptionFilter">Filters tech stacks by description</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter)
        {
            return await _techStackService.GetTechStacksAsync(nameFilter, typeFilter, descriptionFilter); ;
        }

        /// <summary>
        /// Get unique tech stack names
        /// </summary>
        /// <returns></returns>
        [HttpGet("unique")]
        public async Task<List<string>> GetUniqueTechStackNamesAsync()
        {
            return await _techStackService.GetUniqueTechStackNames(); 
        }

        [HttpGet("name/overall/frequency")]
        public async Task<List<KeyFrequency>> GetOverallTechStackNameFrequency()
        {
            return await _techStackService.GetTechStackFrequencyAsync();
        }


        [HttpGet("type/overall/frequency")]
        public async Task<List<KeyFrequency>> GetOverallTechStackTypeFrequency()
        {
            return await _techStackService.GetTechStackTypeFrequencyAsync();
        }
    }
}

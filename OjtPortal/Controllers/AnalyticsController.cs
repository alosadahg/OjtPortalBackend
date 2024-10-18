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
        private readonly IStudentService _studentService;

        public AnalyticsController(ISkillService skillService, ITechStackService techStackService, IStudentService studentService)
        {
            this._skillService = skillService;
            this._techStackService = techStackService;
            this._studentService = studentService;
        }


        /// <summary>
        /// Gets overall skill frequency in all training plans
        /// </summary>
        /// <returns></returns>
        [HttpGet("skill")]
        public async Task<List<KeyFrequency>> GetOverallSKillFrequency()
        {
            return await _skillService.GetSkillFrequencyAsync();
        }

        /// <summary>
        /// Gets overall skill frequency grouped by designation
        /// </summary>
        /// <returns></returns>
        [HttpGet("skill/designation")]
        public async Task<List<GroupKeyFrequency>> GetOverallSKillFrequencyGroupedByDesignation()
        {
            return await _skillService.GetSkillFrquenciesByDesignationAsync();
        }

        /// <summary>
        /// Gets the frequency by techstack name in all training plans
        /// </summary>
        /// <returns></returns>
        [HttpGet("techstack/name")]
        public async Task<List<KeyFrequency>> GetOverallTechStackNameFrequency()
        {
            return await _techStackService.GetTechStackFrequencyAsync();
        }

        /// <summary>
        /// Gets the frequency by techstack type in all training plans
        /// </summary>
        [HttpGet("techstack/type")]
        public async Task<List<KeyFrequency>> GetOverallTechStackTypeFrequency()
        {
            return await _techStackService.GetTechStackTypeFrequencyAsync();
        }

        /// <summary>
        /// Gets the designation frequency with filters
        /// </summary>
        /// <param name="instructorId">Filter student designations by student instructor</param>
        /// <param name="departmentCode">Filter student designation by department</param>
        /// <returns></returns>
        [HttpGet("student/designation")]
        public async Task<List<KeyFrequency>> GetStudentDesignationFrequency(int? instructorId, string? departmentCode)
        {
            return await _studentService.GetStudentDesignationFrequency(instructorId, departmentCode);
        }
    }
}

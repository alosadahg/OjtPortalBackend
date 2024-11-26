using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : OjtPortalBaseController
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            this._companyService = companyService;
        }

        /// <summary>
        /// Get companies with filtering
        /// </summary>
        /// <param name="companyNameFilter">Filters by company name</param>
        /// <param name="countryFilter">Filters by country</param>
        /// <param name="addressStateFilter">Filters by state or province</param>
        /// <param name="cityFilter">Filters by city</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<Company>> GetCompaniesWithFileringAsync(string? companyNameFilter, string? countryFilter, string? addressStateFilter, string? cityFilter)
        {
            return await _companyService.GetCompaniesWithFilteringAsync(companyNameFilter, countryFilter, addressStateFilter, cityFilter);
        }

        /// <summary>
        /// Gets the company by id with mentors
        /// </summary>
        /// <param name="companyId">The company id</param>
        /// <returns></returns>
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanyByIdWithMentorsAsync([Required] int companyId)
        {
            var(result, error) = await _companyService.GetCompanyByIdWithMentorsAsync(companyId);
            if (error != null) MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

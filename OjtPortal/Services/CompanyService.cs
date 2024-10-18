using OjtPortal.Entities;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ICompanyService
    {
        Task<List<Company>> GetCompaniesWithFilteringAsync(string? companyNameFilter, string? countryFilter, string? addressStateFilter, string? cityFilter);
    }

    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepo _companyRepo;

        public CompanyService(ICompanyRepo companyRepo)
        {
            this._companyRepo = companyRepo;
        }

        public async Task<List<Company>> GetCompaniesWithFilteringAsync(string? companyNameFilter, string? countryFilter, string? addressStateFilter, string? cityFilter)
        {
            var companies = await _companyRepo.GetCompaniesAsync();
            if (companyNameFilter != null) companies = companies.Where(c => c.CompanyName.ToLower().Contains(companyNameFilter.ToLower())).ToList();
            if (countryFilter != null) companies = companies.Where(c => c.Address.Country.ToLower().Contains(countryFilter.ToLower())).ToList();
            if (addressStateFilter != null) companies = companies.Where(c => c.Address.State.ToLower().Contains(addressStateFilter.ToLower())).ToList();
            if (cityFilter != null) companies = companies.Where(c => c.Address.City.ToLower().Contains(cityFilter.ToLower())).ToList();
            return companies;
        }
    }
}

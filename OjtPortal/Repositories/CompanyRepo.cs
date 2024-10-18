using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ICompanyRepo
    {
        Task<Company> AddCompanyAsync(Company newCompany);
        Task<bool> IsCompanyExistingAsync(Company newCompany);
        Task<Company?> FindCompanyByNameAsync(string companyName);
        Task<List<Company>> GetCompaniesAsync();
    }

    public class CompanyRepo : ICompanyRepo
    {
        private readonly OjtPortalContext _context;

        public CompanyRepo(OjtPortalContext context)
        {
            this._context = context;
        }
        public async Task<Company> AddCompanyAsync(Company newCompany)
        {
            var exists = IsCompanyExistingAsync(newCompany).Result;
            if (exists) { return await FindCompanyByNameAsync(newCompany.CompanyName) ?? new(); }
            await _context.Companies.AddAsync(newCompany);
            await _context.SaveChangesAsync();
            return newCompany;
        }

        public async Task<bool> IsCompanyExistingAsync(Company newCompany)
        {
            var response = await _context.Companies
                .Where(c => c.CompanyName == newCompany.CompanyName 
                        && c.Address.Country == newCompany.Address.Country
                        && c.Address.State == newCompany.Address.State
                        && c.Address.City == newCompany.Address.City
                        && c.Address.Street == newCompany.Address.Street).FirstOrDefaultAsync();
            return (response!=null);
        }

        public async Task<Company?> FindCompanyByNameAsync(string companyName)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.CompanyName == companyName);
        }

        public async Task<List<Company>> GetCompaniesAsync()
        {
            return await _context.Companies.ToListAsync();
        }
    }
}

using System.Net;
using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ICompanyService
    {
        Task<List<Company>> GetCompaniesWithFilteringAsync(string? companyNameFilter, string? countryFilter, string? addressStateFilter, string? cityFilter);
        Task<(CompanyWithMentorsDto?, ErrorResponseModel?)> GetCompanyByIdWithMentorsAsync(int companyId);
    }

    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepo _companyRepo;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepo companyRepo, IMapper mapper)
        {
            this._companyRepo = companyRepo;
            this._mapper = mapper;
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

        public async Task<(CompanyWithMentorsDto?, ErrorResponseModel?)> GetCompanyByIdWithMentorsAsync(int companyId)
        {
            var company = await _companyRepo.GetCompanyWithMentorsAsync(companyId);
            if (company == null) return (null, new ErrorResponseModel(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("company"), LoggingTemplate.MissingRecordDescription("company", companyId.ToString())));
            var result = _mapper.Map<CompanyWithMentorsDto>(company);
            if (company.Mentors != null)
            {
                var mentors = company.Mentors.ToList();
                var mentorUserList = new List<ExistingUserDto>();
                mentors.ForEach(m =>
                {
                    mentorUserList.Add(_mapper.Map<ExistingUserDto>(m.User));
                });
                result.Mentors = mentorUserList;
            }
            return (result, null);
        }

        /*public async Task<List<GroupKeyFrequency>> GetSkillFrequenciesByCompany()
        {

        }*/
    }
}

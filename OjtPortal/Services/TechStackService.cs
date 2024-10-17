using OjtPortal.Entities;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ITechStackService
    {
        Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter);
    }

    public class TechStackService : ITechStackService
    {
        private readonly ITechStackRepo _techStackRepo;

        public TechStackService(ITechStackRepo techStackRepo)
        {
            this._techStackRepo = techStackRepo;
        }

        public async Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter)
        {
            return await _techStackRepo.GetTechStacksAsync(nameFilter, typeFilter, descriptionFilter);
        }
    }
}

using OjtPortal.Entities;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ITechStackService
    {
        Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter);
        Task<List<TechStack>> GetUniqueTechStacks();
        Task<List<string>> GetUniqueTechStackNames();
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

        public async Task<List<TechStack>> GetUniqueTechStacks()
        {
            return await _techStackRepo.GetUniqueNameTechStacksAsync();
        }

        public async Task<List<string>> GetUniqueTechStackNames()
        {
            var stacks = await _techStackRepo.GetUniqueNameTechStacksAsync();
            var names = new List<string>();
            foreach (var stack in stacks)
            {
                names.Add(stack.Name);
            }
            return names;
        }
    }
}

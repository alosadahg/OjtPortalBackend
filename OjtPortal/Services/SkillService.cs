using OjtPortal.Entities;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter);
        Task<List<Skill>> GetUniqueTechStacks();
        Task<List<string>> GetUniqueTechStackNames();
    }

    public class SkillService : ISkillService
    {
        private readonly ISkillRepo _skillRepo;

        public SkillService(ISkillRepo skillRepo)
        {
            this._skillRepo = skillRepo;
        }

        public async Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter)
        {
            return await _skillRepo.GetSkillsWithFilteringAsync(nameFilter, descriptionFilter);
        }

        public async Task<List<Skill>> GetUniqueTechStacks()
        {
            return await _skillRepo.GetUniqueNameSkillsAsync();
        }

        public async Task<List<string>> GetUniqueTechStackNames()
        {
            var stacks = await _skillRepo.GetUniqueNameSkillsAsync();
            var names = new List<string>();
            foreach (var stack in stacks)
            {
                names.Add(stack.Name);
            }
            return names;
        }
    }
}

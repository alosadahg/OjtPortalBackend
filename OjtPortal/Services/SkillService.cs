using OjtPortal.Entities;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter);
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
    }
}

using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter);
        Task<List<KeyFrequency>> GetSkillFrequencyAsync();
        Task<List<string>> GetUniqueSkillNames();
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

        public async Task<List<KeyFrequency>> GetSkillFrequencyAsync()
        {
            var allSkillNames = await GetUniqueSkillNames();
            var frequencyDictionary = new Dictionary<string, int>();
            foreach (var skillName in allSkillNames) {
                var skills = await _skillRepo.GetSkillsByNameAsync(skillName);

                foreach (var skill in skills)
                {
                    if (skill.Tasks != null && skill.Tasks.Any())
                    {
                        if (frequencyDictionary.ContainsKey(skill.Name))
                        {
                            frequencyDictionary[skill.Name] += skill.Tasks.Count;
                        }
                        else
                        {
                            frequencyDictionary[skill.Name] = skill.Tasks.Count;
                        }
                    }
                } 
            }

            var frequencyList = frequencyDictionary
                .Select(kvp => new KeyFrequency
                {
                    Name = kvp.Key,
                    Usage = kvp.Value
                })
                .OrderByDescending(o => o.Usage)
                .ToList();

            return frequencyList;
        }

        public async Task<List<string>> GetUniqueSkillNames()
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

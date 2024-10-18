using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ITechStackService
    {
        Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter);
        Task<List<TechStack>> GetUniqueTechStacks();
        Task<List<string>> GetUniqueTechStackNames();
        Task<List<string>> GetUniqueTechStackTypes();
        Task<List<KeyFrequency>> GetTechStackFrequencyAsync();
        Task<List<KeyFrequency>> GetTechStackTypeFrequencyAsync();
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

        public async Task<List<string>> GetUniqueTechStackTypes()
        {
            var stacks = await _techStackRepo.GetUniqueTypeTechStacksAsync();
            var names = new List<string>();
            foreach (var stack in stacks)
            {
                names.Add(stack.Type);
            }
            return names;
        }

        public async Task<List<KeyFrequency>> GetTechStackFrequencyAsync()
        {
            var allTechStackNames = await GetUniqueTechStackNames();
            var frequencyDictionary = new Dictionary<string, int>();
            foreach (var techStack in allTechStackNames)
            {
                var teckStacks = await _techStackRepo.GetTechStacksByNameAsync(techStack);

                foreach (var skill in teckStacks)
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
                    Key = kvp.Key,
                    Usage = kvp.Value
                })
                .OrderByDescending(o => o.Usage)
                .ToList();

            return frequencyList;
        }

        public async Task<List<KeyFrequency>> GetTechStackTypeFrequencyAsync()
        {
            var allTechStackNames = await GetUniqueTechStackTypes();
            var frequencyDictionary = new Dictionary<string, int>();
            foreach (var techStackName in allTechStackNames)
            {
                var techStacks = await _techStackRepo.GetTechStacksByTypeAsync(techStackName);

                foreach (var techStack in techStacks)
                {
                    if (techStack.Tasks != null && techStack.Tasks.Any())
                    {
                        if (frequencyDictionary.ContainsKey(techStack.Type))
                        {
                            frequencyDictionary[techStack.Type] += techStack.Tasks.Count;
                        }
                        else
                        {
                            frequencyDictionary[techStack.Type] = techStack.Tasks.Count;
                        }
                    }
                }
            }

            var frequencyList = frequencyDictionary
                .Select(kvp => new KeyFrequency
                {
                    Key = kvp.Key,
                    Usage = kvp.Value
                })
                .OrderByDescending(o => o.Usage)
                .ToList();

            return frequencyList;
        }
    }
}

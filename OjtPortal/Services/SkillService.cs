using Castle.Components.DictionaryAdapter.Xml;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Repositories;
using System.Linq;

namespace OjtPortal.Services
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter);
        Task<List<KeyFrequency>> GetSkillFrequencyAsync();
        Task<List<string>> GetUniqueSkillNames();
        Task<List<GroupKeyFrequency>> GetSkillFrquenciesByDesignationAsync();
    }

    public class SkillService : ISkillService
    {
        private readonly ISkillRepo _skillRepo;
        private readonly ICompanyRepo _companyRepo;
        private readonly ITrainingPlanRepo _trainingPlanRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly ITaskRepo _taskRepo;

        public SkillService(ISkillRepo skillRepo, ICompanyRepo companyRepo, ITrainingPlanRepo trainingPlanRepo, IStudentRepo studentRepo, ITaskRepo taskRepo)
        {
            this._skillRepo = skillRepo;
            this._companyRepo = companyRepo;
            this._trainingPlanRepo = trainingPlanRepo;
            this._studentRepo = studentRepo;
            this._taskRepo = taskRepo;
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
                    Key = kvp.Key,
                    Usage = kvp.Value
                })
                .OrderByDescending(o => o.Usage)
                .ToList();

            return frequencyList;
        }

        public async Task<List<string>> GetUniqueSkillNames()
        {
            var skills = await _skillRepo.GetUniqueNameSkillsAsync();
            var names = new List<string>();
            foreach (var skill in skills)
            {
                names.Add(skill.Name);
            }
            return names;
        }

        /*        public async Task<List<GroupKeyFrequency>> GetSkillsByCompanyAsync()
                {
                    var companies = await _companyRepo.GetCompaniesWithMentorsAsync();
                    foreach(var company in companies)
                    {
                        var mentors = company.Mentors!.ToList();
                        foreach(var mentor in mentors)
                        {
                            var trainingPlans = await _trainingPlanRepo.GetTrainingPlansByMentorAsync(mentor.UserId);
                            foreach(var trainingPlan in trainingPlans.)
                        }
                    }
                }*/

        public async Task<List<GroupKeyFrequency>> GetSkillFrquenciesByDesignationAsync()
        {
            var allSkillNames = (await GetUniqueSkillNames()).Select(name => name.ToLower()).ToList();
            var designationList = await _studentRepo.GetUniqueStudentDesigntionsAsync();

            var response = new List<GroupKeyFrequency>();

            foreach (var designation in designationList)
            {
                var trainingPlans = await _trainingPlanRepo.GetTrainingPlansByDescription(designation);
                var frequencyDictionary = new Dictionary<string, int>();
                foreach (var plan in trainingPlans)
                {
                    var tasks = plan.Tasks;
                    foreach (var task in tasks)
                    {
                        if (task.Skills != null && task.Skills.Any())
                        {
                            foreach (var skill in task.Skills)
                            {
                                var skillName = skill.Name.ToLower();
                                if (allSkillNames.Contains(skillName))
                                {
                                    if (frequencyDictionary.ContainsKey(skillName))
                                    {
                                        frequencyDictionary[skillName]++;
                                    }
                                    else
                                    {
                                        frequencyDictionary[skillName] = 1;
                                    }
                                }
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
                    .OrderByDescending(k => k.Usage)
                    .ToList();

                response.Add(new GroupKeyFrequency
                {
                    GroupedBy = designation,
                    Frequencies = frequencyList
                });
            }

            return response;
        }

    }
}

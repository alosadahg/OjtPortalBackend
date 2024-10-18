using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ISkillRepo
    {
        Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter);
        Task<List<Skill>> GetUniqueNameSkillsAsync();
        Task<List<Skill>> GetUniqueNameSkillsWithTasksAsync();
        Task<List<Skill>> GetSkillsByNameAsync(string name);
    }

    public class SkillRepo : ISkillRepo

    {
        private readonly OjtPortalContext _context;

        public SkillRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter)
        {
            var skills = await _context.Skills.Include(s => s.Tasks).Where(s => s.IsSystemGenerated).ToListAsync();
            if (nameFilter != null) skills = skills.Where(s => s.Name.ToLower().Contains(nameFilter.ToLower())).ToList();
            if (descriptionFilter != null) skills = skills.Where(s => s.Description.ToLower().Contains(descriptionFilter.ToLower())).ToList();

            skills = skills.GroupBy(s => new { Name = s.Name.ToLower(), Description = s.Description.ToLower() })
                        .Select(group => group.First())
                        .ToList();
            
            return skills;
        }

        public async Task<List<Skill>> GetUniqueNameSkillsAsync()
        {
            var skills = await _context.Skills.ToListAsync();
            skills = skills.GroupBy(ts => ts.Name).Select(group => group.First()).OrderBy(ts => ts.Name).ToList();
            return skills;
        }

        public async Task<List<Skill>> GetUniqueNameSkillsWithTasksAsync()
        {
            var skills = await _context.Skills.Include(s => s.Tasks).ToListAsync();
            skills = skills.GroupBy(ts => ts.Name).Select(group => group.First()).OrderBy(ts => ts.Name).ToList();
            return skills;
        }

        public async Task<List<Skill>> GetSkillsByNameAsync(string name)
        {
            return await _context.Skills.Include(s => s.Tasks).Where(sk => sk.Name.ToLower().Equals(name.ToLower())).ToListAsync();
        }
    }
}

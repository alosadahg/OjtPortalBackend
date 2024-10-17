using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ISkillRepo
    {
        Task<List<Skill>> GetSkillsWithFilteringAsync(string? nameFilter, string? descriptionFilter);
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
            var skills = await _context.Skills.Where(s => s.IsSystemGenerated).ToListAsync();
            if (nameFilter != null) skills = skills.Where(s => s.Name.ToLower().Contains(nameFilter.ToLower())).ToList();
            if (descriptionFilter != null) skills = skills.Where(s => s.Description.ToLower().Contains(descriptionFilter.ToLower())).ToList();

            skills = skills.GroupBy(s => new { Name = s.Name.ToLower(), Description = s.Description.ToLower() })
                        .Select(group => group.First())
                        .ToList();
            
            return skills;
        }
    }
}

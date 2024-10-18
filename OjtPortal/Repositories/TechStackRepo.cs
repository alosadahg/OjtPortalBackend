using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ITechStackRepo
    {
        Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter);
        Task<List<TechStack>> GetUniqueNameTechStacksAsync();
        Task<List<TechStack>> GetUniqueTypeTechStacksAsync();
        Task<List<TechStack>> GetTechStacksByNameAsync(string name);
        Task<List<TechStack>> GetTechStacksByTypeAsync(string type);
    }

    public class TechStackRepo : ITechStackRepo
    {
        private readonly OjtPortalContext _context;

        public TechStackRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter)
        {
            var techStacks = await _context.TechStacks.Where(t => t.IsSystemGenerated).ToListAsync();
            if (nameFilter != null) techStacks = techStacks.Where(t => t.Name.ToLower().Contains(nameFilter.ToLower())).ToList();
            if (typeFilter != null) techStacks = techStacks.Where(t => t.Type.ToLower().Contains(typeFilter.ToLower())).ToList();
            if (descriptionFilter != null) techStacks = techStacks.Where(t => t.Description.ToLower().Contains(descriptionFilter.ToLower())).ToList();

            techStacks = techStacks
               .GroupBy(t => new { Name = t.Name.ToLower(), Type = t.Type.ToLower(), Description = t.Description.ToLower() })
               .Select(group => group.First())
               .ToList();

            return techStacks;
        }

        public async Task<List<TechStack>> GetUniqueNameTechStacksAsync()
        {
            var techStacks = await _context.TechStacks.ToListAsync();
            techStacks = techStacks.GroupBy(ts => ts.Name).Select(group => group.First()).OrderBy(ts => ts.Name).ToList();
            return techStacks;
        }

        public async Task<List<TechStack>> GetUniqueTypeTechStacksAsync()
        {
            var techStacks = await _context.TechStacks.ToListAsync();
            techStacks = techStacks.GroupBy(ts => ts.Type).Select(group => group.First()).OrderBy(ts => ts.Type).ToList();
            return techStacks;
        }

        public async Task<List<TechStack>> GetTechStacksByNameAsync(string name)
        {
            return await _context.TechStacks.Include(s => s.Tasks).Where(sk => sk.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }

        public async Task<List<TechStack>> GetTechStacksByTypeAsync(string type)
        {
            return await _context.TechStacks.Include(s => s.Tasks).Where(sk => sk.Type.ToLower().Contains(type.ToLower())).ToListAsync();
        }
    }
}

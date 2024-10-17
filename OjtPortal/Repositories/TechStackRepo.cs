using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ITechStackRepo
    {
        Task<List<TechStack>> GetTechStacksAsync(string? nameFilter, string? typeFilter, string? descriptionFilter);
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
    }
}

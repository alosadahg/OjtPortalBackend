using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IDegreeProgramRepository
    {
        Task<List<DegreeProgram>> GetDegreeProgramsAsync();
    }

    public class DegreeProgramRepository : IDegreeProgramRepository
    {
        private readonly OjtPortalContext _context;

        public DegreeProgramRepository(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<List<DegreeProgram>> GetDegreeProgramsAsync()
        {
            return await _context.DegreePrograms.Include(dp => dp.Department).ToListAsync();
        }
    }
}

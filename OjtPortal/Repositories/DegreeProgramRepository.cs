using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IDegreeProgramRepository
    {
        Task<List<DegreeProgram>> GetDegreeProgramsAsync();
        Task<DegreeProgram?> FindDegreeProgramById(int id);
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

        public async Task<DegreeProgram?> FindDegreeProgramById(int id)
        {
            return await _context.DegreePrograms.Include(dp => dp.Department).FirstOrDefaultAsync(dp => dp.Id == id);
        }

        public async Task<DegreeProgram?> FindDegreeProgramByDepartmentId(int id)
        {
            return await _context.DegreePrograms.Include(dp => dp.Department).FirstOrDefaultAsync(dp => dp.DepartmentId == id);
        }
    }
}

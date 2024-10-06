using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IDegreeProgramRepo
    {
        Task<List<DegreeProgram>> GetDegreeProgramsAsync();
        Task<DegreeProgram?> FindDegreeProgramById(int id);
        Task<List<DegreeProgram>> FindByDepartmentId(int id);
    }

    public class DegreeProgramRepo : IDegreeProgramRepo
    {
        private readonly OjtPortalContext _context;

        public DegreeProgramRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<List<DegreeProgram>> GetDegreeProgramsAsync()
        {
            return await _context.DegreePrograms.Include(dp => dp.Department).ToListAsync();
        }

        public async Task<DegreeProgram?> FindDegreeProgramById(int id)
        {
            return await _context.DegreePrograms.Include(dp => dp.Department).Include(dp => dp.Department.Students).FirstOrDefaultAsync(dp => dp.Id == id);
        }

        public async Task<List<DegreeProgram>> FindByDepartmentId(int id)
        {
            return await _context.DegreePrograms.Include(dp => dp.Department).Where(dp => dp.DepartmentId == id).ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IDepartmentRepo
    {
        Task<List<Department>> GetDepartmentsAsync();
        Task<Department?> FindByDepartmentIdAsync(int id);
    }

    public class DepartmentRepo : IDepartmentRepo
    {
        private readonly OjtPortalContext _context;

        public DepartmentRepo(OjtPortalContext context)
        {
            this._context = context;
        }
        
        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department?> FindByDepartmentIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }
    }
}

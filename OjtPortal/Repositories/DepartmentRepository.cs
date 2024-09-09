using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetDepartmentsAsync();
    }

    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly OjtPortalContext _context;

        public DepartmentRepository(OjtPortalContext context)
        {
            this._context = context;
        }
        
        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }
    }
}

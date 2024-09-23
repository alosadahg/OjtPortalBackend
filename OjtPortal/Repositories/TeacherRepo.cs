using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ITeacherRepo
    {
        Task<Teacher?> AddTeacherAsync(Teacher newTeacher);
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task<bool> IsTeacherExisting(Teacher instructor);
    }

    public class TeacherRepo : ITeacherRepo
    {
        private readonly OjtPortalContext _context;

        public TeacherRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Teacher?> AddTeacherAsync(Teacher newTeacher)
        {
            if (IsTeacherExisting(newTeacher).Result) return null;
            _context.Entry(newTeacher.Department).State = EntityState.Unchanged;
            await _context.Teachers.AddAsync(newTeacher);
            await _context.SaveChangesAsync();
            return newTeacher;
        }

        public async Task<bool> IsTeacherExisting(Teacher instructor)
        {
            return await _context.Teachers.ContainsAsync(instructor);
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }
    }
}

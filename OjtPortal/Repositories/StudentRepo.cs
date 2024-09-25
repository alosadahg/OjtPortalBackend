using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IStudentRepo
    {
        Task<Student?> AddStudentAsync(Student newStudent);
        Task<Student?> GetStudentByIdAsync(int id, bool includeUser);
        Task<Student?> GetStudentBySchoolIdAsync(string id);
        Task<bool> IsStudentExistingAsync(Student student);
    }

    public class StudentRepo : IStudentRepo
    {
        private readonly OjtPortalContext _context;

        public StudentRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Student?> AddStudentAsync(Student newStudent)
        {
            if (await _context.Students.FirstOrDefaultAsync(s => s.StudentId == newStudent.StudentId) != null) return null;
            if (IsStudentExistingAsync(newStudent).Result) return null;
            await _context.Students.AddAsync(newStudent);
            _context.SaveChanges();
            return newStudent;
        }

        public async Task<Student?> GetStudentByIdAsync(int id, bool includeUser)
        {
            var query = (includeUser) ?
                await _context.Students
                .Include(s => s.DegreeProgram)
                .Include(s => s.DegreeProgram.Department)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == id) :
                await _context.Students
                .Include(s => s.DegreeProgram)
                .Include(s => s.DegreeProgram.Department)
                .FirstOrDefaultAsync(s => s.UserId == id);

            return query;
        }

        public async Task<Student?> GetStudentBySchoolIdAsync(string id)
        {
            return await _context.Students
                .Include(s => s.DegreeProgram)
                .Include(s => s.DegreeProgram.Department)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<bool> IsStudentExistingAsync(Student student)
        {
            return await _context.Students.ContainsAsync(student);
        }
    }
}

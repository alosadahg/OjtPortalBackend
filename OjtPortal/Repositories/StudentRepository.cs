using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> AddStudentAsync(Student newStudent);
        Task<Student?> GetStudentByIdAsync(int id);
        Task<Student?> GetStudentBySchoolIdAsync(string id);
        Task<bool> IsStudentExistingAsync(Student student);
    }

    public class StudentRepository : IStudentRepository
    {
        private readonly OjtPortalContext _context;

        public StudentRepository(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Student?> AddStudentAsync(Student newStudent)
        {
            if (IsStudentExistingAsync(newStudent).Result) return null;
            await _context.Students.AddAsync(newStudent);
            _context.SaveChanges();
            return newStudent;
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Instructor)
                .Include(s => s.Mentor)
                .Include(s => s.DegreeProgram)
                .FirstOrDefaultAsync(s => s.UserId == id);
        }

        public async Task<Student?> GetStudentBySchoolIdAsync(string id)
        {
            return await _context.Students
                .Include(s => s.Instructor)
                .Include(s => s.Mentor)
                .Include(s => s.DegreeProgram)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<bool> IsStudentExistingAsync(Student student)
        {
            return await _context.Students.ContainsAsync(student);
        }
    }
}

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
        private readonly ILogger<StudentRepo> _logger;

        public StudentRepo(OjtPortalContext context, ILogger<StudentRepo> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<Student?> AddStudentAsync(Student newStudent)
        {
            try
            {
                if (await IsStudentExistingAsync(newStudent)) return null;
                await _context.Students.AddAsync(newStudent);
                await _context.SaveChangesAsync();
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
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
            var containsObject = await _context.Students.ContainsAsync(student);
            if(!containsObject)
            {
                containsObject = (await _context.Students
                    .Where(s=> s.StudentId.Equals(student.StudentId))
                    .FirstOrDefaultAsync() != null) ? true : false;
            }
            return containsObject;
        }
    }
}

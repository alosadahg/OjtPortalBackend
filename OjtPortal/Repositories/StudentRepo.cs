using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;

namespace OjtPortal.Repositories
{
    public interface IStudentRepo
    {
        Task<Student?> AddStudentAsync(Student newStudent);
        Task<Student?> GetStudentByIdAsync(int id, bool includeUser);
        Task<Student?> GetStudentBySchoolIdAsync(string id);
        Task<bool> IsStudentExistingAsync(Student student);
        Task<Student?> UpdateStudentByMentorAsync(Student student, int id);
        Task<Student?> UpdateStudentInfoAsync(Student student);
    }

    public class StudentRepo : IStudentRepo
    {
        private readonly OjtPortalContext _context;
        private readonly ILogger<StudentRepo> _logger;
        private readonly UserManager<User> _userManager;

        public StudentRepo(OjtPortalContext context, ILogger<StudentRepo> logger, UserManager<User> userManager)
        {
            this._context = context;
            this._logger = logger;
            this._userManager = userManager;
        }

        public async Task<Student?> AddStudentAsync(Student newStudent)
        {
            try
            {
                if (await IsStudentExistingAsync(newStudent)) return null;
                await _context.Students.AddAsync(newStudent);
                await _context.SaveChangesAsync();
            }  catch(Exception ex)
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
                .Include(s => s.DegreeProgram!.Department)
                .Include(s => s.User)
                .Include(s => s.Mentor)
                .Include(s => s.Mentor!.User)
                .Include(s => s.Mentor!.Company)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(s => s.UserId == id) :
                await _context.Students
                .Include(s => s.DegreeProgram)
                .Include(s => s.DegreeProgram!.Department)
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
                if (!string.IsNullOrEmpty(student.StudentId))
                {
                    containsObject = (await _context.Students
                        .Where(s => s.StudentId.Equals(student.StudentId))
                        .FirstOrDefaultAsync() != null) ? true : false;
                }
            }
            return containsObject;
        }

        public async Task<Student?> UpdateStudentByMentorAsync(Student student, int id)
        {
            var existingStudent = await GetStudentByIdAsync(id, true);
            if (existingStudent == null) return null;
            if (existingStudent.MentorId == null) existingStudent.MentorId = student.MentorId;
            existingStudent.Designation = student.Designation;
            existingStudent.Division = student.Division;
            existingStudent.StartDate = student.StartDate;
            existingStudent.EndDate = student.EndDate;
            existingStudent.HrsToRender = student.HrsToRender;
            existingStudent.Shift = student.Shift;
            _context.Entry(existingStudent.User!).State = EntityState.Unchanged;
            if(existingStudent.Instructor != null) _context.Entry(existingStudent.Instructor).State = EntityState.Unchanged;
            if(existingStudent.DegreeProgram != null) _context.Entry(existingStudent.DegreeProgram!).State = EntityState.Unchanged;
            await _context.SaveChangesAsync();
            return existingStudent;
        }

        public async Task<Student?> UpdateStudentInfoAsync(Student student)
        {
            var existingStudent = await GetStudentByIdAsync(student.User!.Id, true);
            if (existingStudent == null) return null;
            if (!string.IsNullOrEmpty(student.User.Email))
            {
                existingStudent.User!.Email = student.User!.Email;
                existingStudent.User.NormalizedEmail = _userManager.NormalizeEmail(student.User.Email);
                await _userManager.SetUserNameAsync(existingStudent.User!, student.User.Email!);
                existingStudent.User.AccountStatus = AccountStatus.Pending;
            }

            existingStudent.InstructorId = student.InstructorId;
            existingStudent.StudentId = student.StudentId;
            existingStudent.User!.FirstName = student.User!.FirstName;
            existingStudent.User!.LastName = student.User!.LastName;
            existingStudent.DegreeProgramId = student.DegreeProgram!.Id;
            student.DegreeProgram.Department.Students!.Add(existingStudent);

            if(existingStudent.Shift != null) _context.Entry(existingStudent.Shift).State = EntityState.Unchanged;
            if(existingStudent.Mentor != null) _context.Entry(existingStudent.Mentor).State = EntityState.Unchanged;
            await _context.SaveChangesAsync();
            return existingStudent;
        }
    }
}

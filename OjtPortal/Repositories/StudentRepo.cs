using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel;

namespace OjtPortal.Repositories
{
    public interface IStudentRepo
    {
        Task<Student?> AddStudentAsync(Student newStudent);
        Task<Student?> GetStudentByIdAsync(int id, bool includeMentor, bool includeInstructor, bool includeAttendance);
        Task<List<Student>> GetAllStudentsAsync(bool includeMentor, bool includeInstructor, bool includeAttendance);
        Task<Student?> GetStudentBySchoolIdAsync(string id);
        Task<bool> IsStudentExistingAsync(Student student);
        Task<Student?> UpdateStudentByMentorAsync(Student student, int id);
        Task<Student?> UpdateStudentByTeacherAsync(Student student, int id);
        Task<Student?> UpdateStudentInfoAsync(Student student);
        Task<Student?> UpdateStudentInternshipStatusAsync(Student student, InternshipStatus status);
        Task<Student?> UpdateStudentEndDateAsync(Student student, DateOnly newEndDate);
        Task<Student?> UpdateStudentAbsentCountAsync(Student student, int addedAbsent);
        Task<List<Student>?> GetStudentsForTrainingPlanAsync();
        Task<List<string>> GetUniqueStudentDesigntionsAsync();
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
                if (string.IsNullOrWhiteSpace(newStudent.Designation)) newStudent.Designation = string.Empty;
                if (string.IsNullOrWhiteSpace(newStudent.Division)) newStudent.Division = string.Empty;
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

        public async Task<Student?> GetStudentByIdAsync(int id, bool includeMentor, bool includeInstructor, bool includeAttendance)
        {
            IQueryable<Student> query = _context.Students
                .Include(s => s.DegreeProgram)
                .ThenInclude(dp => dp.Department)
                .Include(s => s.User);

            if (includeMentor) query = query.Include(s => s.Mentor).ThenInclude(m => m.User).Include(s => s.Mentor!.Company);
            if (includeInstructor) query = query.Include(s => s.Instructor).ThenInclude(i => i.User);
            if (includeAttendance) query = query.Include(s => s.Attendances).ThenInclude(a => a.LogbookEntry);

            return await query.FirstOrDefaultAsync(s => s.UserId == id);
        }

        public async Task<List<Student>> GetAllStudentsAsync(bool includeMentor, bool includeInstructor, bool includeAttendance)
        {
            IQueryable<Student> query = _context.Students
                .Include(s => s.DegreeProgram)
                .ThenInclude(dp => dp.Department)
                .Include(s => s.User);

            if (includeMentor) query = query.Include(s => s.Mentor).ThenInclude(m => m.User).Include(s => s.Mentor!.Company);
            if (includeInstructor) query = query.Include(s => s.Instructor).ThenInclude(i => i.User);
            if (includeAttendance) query = query.Include(s => s.Attendances).ThenInclude(a => a.LogbookEntry);

            return await query.ToListAsync();
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
            var containsObject = (!string.IsNullOrEmpty(student.StudentId)) 
                ? (await _context.Students.FirstOrDefaultAsync(s => s.StudentId == student.StudentId) != null)
                : await _context.Students.ContainsAsync(student);
            return containsObject;
        }

        public async Task<Student?> UpdateStudentByMentorAsync(Student student, int id)
        {
            var existingStudent = await GetStudentByIdAsync(id, true, true, false);
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
            var existingStudent = await GetStudentByIdAsync(student.User!.Id, false, true, false);
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

        public async Task<Student?> UpdateStudentByTeacherAsync(Student student, int id)
        {
            var existingStudent = await GetStudentByIdAsync(id, false, true, false);
            if (existingStudent == null) return null;
            if (existingStudent.InstructorId == null) existingStudent.InstructorId = student.InstructorId;
            
            if(string.IsNullOrEmpty(existingStudent.StudentId)) existingStudent.StudentId = student.StudentId;
            if (existingStudent.DegreeProgramId == null)
            {
                existingStudent.DegreeProgramId = student.DegreeProgram!.Id;
                student.DegreeProgram.Department.Students!.Add(existingStudent);
            }

            if (existingStudent.User != null) _context.Entry(existingStudent.User).State = EntityState.Unchanged;
            if (existingStudent.Shift != null) _context.Entry(existingStudent.Shift).State = EntityState.Unchanged;
            if (existingStudent.Mentor != null) _context.Entry(existingStudent.Mentor).State = EntityState.Unchanged;
            await _context.SaveChangesAsync();
            return existingStudent;
        }

        public async Task<Student?> UpdateStudentInternshipStatusAsync(Student student, InternshipStatus status)
        {
            student.InternshipStatus = status;
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student?> UpdateStudentEndDateAsync(Student student, DateOnly newEndDate)
        {
            student.EndDate = newEndDate;
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student?> UpdateStudentAbsentCountAsync(Student student, int addedAbsent)
        {
            student.Shift!.AbsencesCount += addedAbsent;
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<List<Student>?> GetStudentsForTrainingPlanAsync()
        {
            var students = await _context.Students
                .Include(s => s.DegreeProgram)
                .ThenInclude(dp => dp.Department)
                .Include(s => s.User)
                .ToListAsync();

            var uniqueStudents = students
                .DistinctBy(s => new { s.Division, s.Designation, s.HrsToRender, s.Shift.DailyDutyHrs })
                .ToList();

            return uniqueStudents;
        }

        public async Task<List<string>> GetUniqueStudentDesigntionsAsync()
        {
            var students = await _context.Students.ToListAsync();
            students = students.DistinctBy(s => s.Designation).ToList();
            var designationList = new List<string>();
            foreach (var student in students) if (!string.IsNullOrEmpty(student.Designation)) designationList.Add(student.Designation);
            return designationList;
        }
    }
}

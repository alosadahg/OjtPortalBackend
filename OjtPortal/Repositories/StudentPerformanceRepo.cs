using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IStudentPerformanceRepo
    {
        Task<StudentPerformance> AddStudentPerformance(StudentPerformance studentPerformance);
        Task<StudentPerformance?> GetPerformanceByIdAsync(int studentId);
        Task<StudentPerformance> UpdateStudentPerformance(StudentPerformance existing, StudentPerformance updated);
    }

    public class StudentPerformanceRepo : IStudentPerformanceRepo
    {
        private readonly OjtPortalContext _context;

        public StudentPerformanceRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<StudentPerformance> AddStudentPerformance(StudentPerformance studentPerformance)
        {
            _context.StudentPerformances.Add(studentPerformance);
            await _context.SaveChangesAsync();
            return studentPerformance;
        }

        public async Task<StudentPerformance?> GetPerformanceByIdAsync(int studentId)
        {
            var student = await _context.StudentPerformances.Include(sp => sp.Student).FirstOrDefaultAsync(sp => sp.StudentId == studentId);
            return student;
        }

        public async Task<StudentPerformance> UpdateStudentPerformance(StudentPerformance existing, StudentPerformance updated)
        {
            existing.PerformanceStatus = updated.PerformanceStatus;
            existing.AttendanceCount = updated.AttendanceCount;
            existing.LogbookCount = updated.LogbookCount;
            existing.RemainingHoursToRender = updated.RemainingHoursToRender;
            existing.RemainingManDays = updated.RemainingManDays;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}

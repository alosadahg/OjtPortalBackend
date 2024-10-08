using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IAttendanceRepo
    {
        Task<Attendance?> GetAttendanceByDateAsync(DateOnly date, int id);
        Task<Attendance?> GetAttendanceByIdAsync(long id);
        Task<Attendance?> AddAttendanceAsync(Attendance attendance);
        Attendance? GetRecentAttendance(Student student);
        Task<Attendance?> TimeOutAsync(Attendance attendance);
    }

    public class AttendanceRepo : IAttendanceRepo
    {
        private readonly OjtPortalContext _context;

        public AttendanceRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Attendance?> GetAttendanceByDateAsync(DateOnly date, int id)
        {
            var attendance = await _context.Attendances.Where(a => a.StudentId == id && (DateOnly.FromDateTime(a.TimeIn)).Equals(date)).FirstOrDefaultAsync();
            return attendance!;
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(long id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            return attendance;
        }

        public async Task<Attendance?> AddAttendanceAsync(Attendance attendance)
        {
            // time in is 15 mins late
            if (TimeOnly.FromDateTime(attendance.TimeIn) - attendance.Student.Shift!.Start > TimeSpan.FromMinutes(15)) attendance.Student.Shift.LateTimeInCount++;
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public Attendance? GetRecentAttendance(Student student)
        {
            var attendanceList = student.Attendances;
            if (attendanceList == null || attendanceList.Any() == false) return null;
            var sortedAttendance = attendanceList.OrderByDescending(a => a.AttendanceId).ToList();
            var attendance = sortedAttendance.FirstOrDefault();
            return attendance;
        }

        public async Task<Attendance?> TimeOutAsync(Attendance attendance)
        {
            attendance.TimeOut = DateTime.UtcNow;
            if (TimeOnly.FromDateTime(attendance.TimeOut.Value) - attendance.Student.Shift!.End > TimeSpan.FromHours(1))
            {
                attendance.IsTimeOutLate = true;
                attendance.Student.Shift.LateTimeOutCount++;
            }
            var span = attendance.TimeOut - attendance.TimeIn;
            attendance.RenderedHours = (span.Value.TotalHours > attendance.Student.Shift.DailyDutyHrs) ? 
                attendance.Student.Shift.DailyDutyHrs : span.Value.TotalHours;
            attendance.Student.Shift!.TotalHrsRendered += attendance.RenderedHours;
            attendance.Student.Shift!.TotalManDaysRendered++;
            await _context.SaveChangesAsync();
            return attendance;
        }
    }
}

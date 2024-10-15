using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;

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
            var attendance = await _context.Attendances.Include(a => a.LogbookEntry).FirstOrDefaultAsync(a => a.AttendanceId == id);
            return attendance;
        }

        public async Task<Attendance?> AddAttendanceAsync(Attendance attendance)
        {
            // time in is 15 mins late
            if (TimeOnly.FromDateTime(DateTime.Now) - attendance.Student.Shift!.Start > TimeSpan.FromMinutes(15))
            {
                attendance.Student.Shift.LateTimeInCount++;
                attendance.IsTimeInLate = true;
            }
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
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var end = attendance.Student.Shift!.End;
            var timeSpan = currentTime.Hour - attendance.Student.Shift!.End!.Value.Hour;
            var lastDate = DateOnly.FromDateTime(UtcDateTimeHelper.FromUtcToLocal(attendance.TimeIn));
            if (TimeSpan.FromHours(timeSpan) > TimeSpan.FromHours(1) || lastDate!=currentDate)
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

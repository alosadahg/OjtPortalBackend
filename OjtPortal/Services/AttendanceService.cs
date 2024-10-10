using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IAttendanceService
    {
        Task<(AttendanceDto?, ErrorResponseModel?)> TimeInAsync(int id, bool proceedTimeIn);
        Task<(AttendanceDto?, ErrorResponseModel?)> GetAttendanceById(int id);
        Task<(AttendanceDto?, ErrorResponseModel?)> TimeOutAsync(int id);
        Task<(string?, ErrorResponseModel?)> IsDateAWorkDay(DateOnly date, Shift shift);
        Task<(List<AttendanceDto>?, ErrorResponseModel?)> GetAttendanceHistoryByStudentAsync(int studentId, DateOnly? start, DateOnly? end, bool? isLateTimeIn, bool? isLateTimeOut);
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly IStudentRepo _studentRepo;
        private readonly IAttendanceRepo _attendanceRepo;
        private readonly IHolidayService _holidayService;
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public AttendanceService(IStudentRepo studentRepo, IAttendanceRepo attendanceRepo, IHolidayService holidayService, IStudentService studentService, IMapper mapper)
        {
            this._studentRepo = studentRepo;
            this._attendanceRepo = attendanceRepo;
            this._holidayService = holidayService;
            this._studentService = studentService;
            this._mapper = mapper;
        }

        public async Task<(AttendanceDto?, ErrorResponseModel?)> TimeInAsync(int id, bool proceedTimeIn)
        {
            var student = await _studentRepo.GetStudentByIdAsync(id, false, false, true);
            if (student == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", id.ToString())));
            var attendance = new Attendance
            {
                Student = student,
                TimeIn = DateTime.UtcNow,
            };
            if (student.Shift == null) return (null, new(HttpStatusCode.BadRequest, LoggingTemplate.MissingRecordTitle("shift"), LoggingTemplate.MissingRecordDescription("shift", id.ToString())));
            var dateToday = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local));

            #region Check if today is a workday
            // date checking
            if (!proceedTimeIn)
            {
                var (isWorkDay, error) = await IsDateAWorkDay(dateToday, student.Shift);
                if (error != null) return (null, error);
            }
            #endregion

            #region Check if there is no clock in yet and update absences
            var recentAttendance = _attendanceRepo.GetRecentAttendance(student);

            if (recentAttendance != null)
            {
                // convert utc to local 
                var recentTimeIn = TimeZoneInfo.ConvertTimeFromUtc(recentAttendance.TimeIn, TimeZoneInfo.Local);
                if (DateOnly.FromDateTime(recentAttendance!.TimeIn).Equals(dateToday)) return (null, new(HttpStatusCode.Conflict, "Time in already recorded", "Today's time in is already recorded."));
                if (recentAttendance.TimeOut == null) return (null, new(HttpStatusCode.Conflict, "Recent attendance not yet clocked out", "Has not clocked out yet from previous attendance, please clock out."));
                var lastTimeInDate = DateOnly.FromDateTime(recentTimeIn);
                // TODO: absent checking
                var absencesCount = await GetAbsentCountAsync(lastTimeInDate, student);
                student.Shift.AbsencesCount += absencesCount;
            }
            
            #endregion

            #region Check if time is valid
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            var earlyTimeIn = student.Shift!.Start!.Value.AddMinutes(-15);

            // current time is before allowed time in
            if (currentTime < earlyTimeIn) return (null, new(HttpStatusCode.BadRequest, "Time in not yet allowed", $"You can time in later on {earlyTimeIn}"));

            // current time is after shift, time in not allowed
            if (currentTime >= student.Shift.End!.Value) return (null, new(HttpStatusCode.BadRequest, "Time in not allowed", "Today's shift has already ended."));
            #endregion

            if (student.InternshipStatus.Equals(InternshipStatus.Pending)) await _studentRepo.UpdateStudentInternshipStatusAsync(student, InternshipStatus.Ongoing);
            await _attendanceRepo.AddAttendanceAsync(attendance);
            return (_mapper.Map<AttendanceDto>(attendance), null);
        }

        public async Task<(AttendanceDto?, ErrorResponseModel?)> GetAttendanceById(int id)
        {
            var attendance = await _attendanceRepo.GetAttendanceByIdAsync(id);
            if (attendance == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("attendance"), LoggingTemplate.MissingRecordDescription("attendance", id.ToString())));
            return (_mapper.Map<AttendanceDto>(attendance), null);
        }

        public async Task<(AttendanceDto?, ErrorResponseModel?)> TimeOutAsync(int id)
        {
            var student = await _studentRepo.GetStudentByIdAsync(id, false, false, true);
            if (student == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", id.ToString())));
            if (student.Shift == null) return (null, new(HttpStatusCode.BadRequest, LoggingTemplate.MissingRecordTitle("shift"), LoggingTemplate.MissingRecordDescription("shift", id.ToString())));
           
            var recentAttendance = _attendanceRepo.GetRecentAttendance(student);
            if (recentAttendance == null || recentAttendance!.TimeOut != null) return (null, new(HttpStatusCode.Conflict, "No recent clock in", "Has not clocked in yet, please clock in first."));

            recentAttendance = await _attendanceRepo.TimeOutAsync(recentAttendance);

            var remainingHrs = student.HrsToRender - student.Shift.TotalHrsRendered;
            var manDays = (int) Math.Ceiling(remainingHrs / student.Shift.DailyDutyHrs);
            var (endDate, _) = await _studentService.GetEndDateAsync(DateOnly.FromDateTime(DateTime.Now), manDays, student.Shift.IncludePublicPhHolidays, student.Shift.WorkingDays);
            student = await _studentRepo.UpdateStudentEndDateAsync(student, endDate!.Value);

            return (_mapper.Map<AttendanceDto>(recentAttendance), null);
        }

        public async Task<(string?, ErrorResponseModel?)> IsDateAWorkDay(DateOnly date, Shift shift)
        {
            if (await _holidayService.IsDateAHoliday(date))
            {
                if (!shift.IncludePublicPhHolidays)
                {
                    return (null, new(HttpStatusCode.UnprocessableContent, "Holiday Off", "Today is a public holiday, if you wish to proceed, please set proceedTimeIn to true."));
                }
            }

            if (date.DayOfWeek == DayOfWeek.Saturday && shift.WorkingDays == WorkingDays.WeekdaysOnly) return (null, new(HttpStatusCode.UnprocessableContent, "Weekend Off", "Today is a weekend, if you wish to proceed, please set proceedTimeIn to true."));
            if (date.DayOfWeek == DayOfWeek.Sunday && shift.WorkingDays != WorkingDays.WholeWeek) return (null, new(HttpStatusCode.UnprocessableContent, "Sunday Off", "Today is a Sunday, if you wish to proceed, please set proceedTimeIn to true."));
            return ($"{date} is a workday.", null);
        }

        public async Task<int> GetAbsentCountAsync(DateOnly recentDate, Student student)
        {
            var shift = student.Shift!;
            var skips = DateOnly.FromDateTime(DateTime.Now).DayNumber - recentDate.DayNumber;
            var absencesCount = 0;
            while (skips > 1)
            {
                var (_, error) = await IsDateAWorkDay(recentDate, shift);
                if (error == null) absencesCount++;
                recentDate = recentDate.AddDays(1);
                skips--;
            }
            return absencesCount;
        }

        public async Task<(List<AttendanceDto>?, ErrorResponseModel?)> GetAttendanceHistoryByStudentAsync(int studentId, DateOnly? start, DateOnly? end, bool? isLateTimeIn, bool? isLateTimeOut)
        {
            var student = await _studentRepo.GetStudentByIdAsync(studentId, false, false, true);
            if (student == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", $"{studentId}")));

            var attendanceList = (student.Attendances != null) ? student.Attendances.ToList() : new();
            var attendanceDtoList = new List<AttendanceDto>();

            attendanceList = attendanceList.OrderByDescending(a => a.AttendanceId).ToList();
            if (isLateTimeIn != null) attendanceList = attendanceList.Where(a => a.IsTimeInLate == isLateTimeIn).ToList();
            if (isLateTimeOut != null) attendanceList = attendanceList.Where(a => a.IsTimeOutLate == isLateTimeOut).ToList();
            if (start != null) attendanceList = attendanceList.Where(a => DateOnly.FromDateTime(UtcDateTimeHelper.FromUtcToLocal(a.TimeIn)) >= start).ToList();
            if (end != null) attendanceList = attendanceList.Where(a => DateOnly.FromDateTime(UtcDateTimeHelper.FromUtcToLocal(a.TimeIn)) <= end).ToList();
            
            attendanceList.ForEach(a => attendanceDtoList.Add(_mapper.Map<AttendanceDto>(a)));
            return (attendanceDtoList, null);
        }
    }
}

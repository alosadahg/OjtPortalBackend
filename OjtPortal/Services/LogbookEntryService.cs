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
    public interface ILogbookEntryService
    {
        Task<(LogbookDto?, ErrorResponseModel?)> AddLogbookEntry(NewLogbookEntryDto newLogbook, int userId);
        Task<(LogbookDto?, ErrorResponseModel?)> GetLogbookByIdAsync(long logbookId);
        Task<(LogbookDto?, ErrorResponseModel?)> AddRemarksAsync(long logbookId, int mentorId, string remarks);
    }

    public class LogbookEntryService : ILogbookEntryService
    {
        private readonly IMapper _mapper;
        private readonly IAttendanceRepo _attendanceRepo;
        private readonly ILogbookEntryRepo _logbookEntryRepo;
        private readonly IStudentRepo _studentRepo;

        public LogbookEntryService(IMapper mapper, IAttendanceRepo attendanceRepo, ILogbookEntryRepo logbookEntryRepo, IStudentRepo _studentRepo)
        {
            this._mapper = mapper;
            this._attendanceRepo = attendanceRepo;
            this._logbookEntryRepo = logbookEntryRepo;
            this._studentRepo = _studentRepo;
        }

        public async Task<(LogbookDto?, ErrorResponseModel?)> AddLogbookEntry(NewLogbookEntryDto newLogbook, int userId)
        {
            var logbook = _mapper.Map<LogbookEntry>(newLogbook);
            var attendance = await _attendanceRepo.GetAttendanceByIdAsync(newLogbook.AttendanceId);
            if (attendance == null) return (null, new(HttpStatusCode.BadRequest, LoggingTemplate.MissingRecordTitle("attendance"), LoggingTemplate.MissingRecordDescription("attendance", newLogbook.AttendanceId.ToString())));

            if (attendance.StudentId != userId) return (null, new(HttpStatusCode.Forbidden, "Invalid User", "Access Not Allowed"));
            if (attendance.LogbookEntry != null) return (null, new(HttpStatusCode.UnprocessableEntity, "Logbook Exists", "Logbook Entry is already recorded for this attendance."));

            var status = (attendance!.IsTimeOutLate) ? LogbookStatus.Pending : LogbookStatus.Submitted;
            logbook.CreationTimestamp = DateTime.UtcNow;
            if (status == LogbookStatus.Submitted) logbook.SubmissionTimestamp = DateTime.UtcNow;
            logbook.Attendance = attendance;
            logbook = await _logbookEntryRepo.AddLogbookEntryAsync(logbook);
            return (_mapper.Map<LogbookDto>(logbook), null);
        }

        public async Task<(LogbookDto?, ErrorResponseModel?)> GetLogbookByIdAsync(long logbookId)
        {
            var logbook = await _logbookEntryRepo.GetLogbookByIdAsync(logbookId);
            if (logbook == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("logbook entry"), LoggingTemplate.MissingRecordDescription("logbook entry", $"{logbookId}")));
            return (_mapper.Map<LogbookDto>(logbook), null);
        }

        public async Task<(LogbookDto?, ErrorResponseModel?)> AddRemarksAsync(long logbookId, int mentorId, string remarks)
        {
            var key = "logbook entry";
            var logbook = await _logbookEntryRepo.GetLogbookByIdAsync(logbookId);
            if (logbook == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, logbookId.ToString())));

            var student = await _studentRepo.GetStudentByIdAsync(logbook.Attendance.StudentId, true, false, false);
            if (student!.MentorId != mentorId) return (null, new(HttpStatusCode.Forbidden, "Forbidden Request", $"Access is not given to this mentor: {mentorId}"));

            if (!string.IsNullOrEmpty(logbook.Remarks)) return (null, new(HttpStatusCode.UnprocessableContent, "Remarks Already Recorded", "This logbook entry has already been given remarks"));
            if (logbook.LogbookStatus != LogbookStatus.Submitted) logbook.LogbookStatus = LogbookStatus.Submitted;

            logbook = await _logbookEntryRepo.AddRemarksAsync(logbook, remarks);
            return (_mapper.Map<LogbookDto>(logbook), null);
        }
    }
}

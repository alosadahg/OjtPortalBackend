using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        Task<(List<LogbookDto>?, ErrorResponseModel?)> GetLogbooksByStudentWithFilteringAsync(int studentId, LogbookStatus? status, DateOnly? startDate, DateOnly? endDate);
        Task<(List<LogbookDto>?, ErrorResponseModel?)> GetLogbooksByMentorWithFilteringAsync(int mentorId, LogbookStatus? status, DateOnly? startDate, DateOnly? endDate);
    }

    public class LogbookEntryService : ILogbookEntryService
    {
        private readonly IMapper _mapper;
        private readonly IAttendanceRepo _attendanceRepo;
        private readonly ILogbookEntryRepo _logbookEntryRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IMentorRepo _mentorRepo;

        public LogbookEntryService(IMapper mapper, IAttendanceRepo attendanceRepo, ILogbookEntryRepo logbookEntryRepo, IStudentRepo studentRepo, IMentorRepo mentorRepo)
        {
            this._mapper = mapper;
            this._attendanceRepo = attendanceRepo;
            this._logbookEntryRepo = logbookEntryRepo;
            this._studentRepo = studentRepo;
            this._mentorRepo = mentorRepo;
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

            if (!string.IsNullOrEmpty(logbook.Remarks)) return (null, new(HttpStatusCode.UnprocessableContent, "LogbookStatusRemarks Already Recorded", "This logbook entry has already been given remarks"));
            if (logbook.LogbookStatus != LogbookStatus.Submitted) logbook.LogbookStatus = LogbookStatus.Submitted;

            logbook = await _logbookEntryRepo.AddRemarksAsync(logbook, remarks);
            return (_mapper.Map<LogbookDto>(logbook), null);
        }

        public async Task<(List<LogbookDto>?, ErrorResponseModel?)> GetLogbooksByStudentWithFilteringAsync(int studentId, LogbookStatus? status, DateOnly? startDate, DateOnly? endDate)
        {
            var key = "student";
            var student = await _studentRepo.GetStudentByIdAsync(studentId, false, false, true);
            if (student == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, studentId.ToString())));
            var attendanceList = (student.Attendances!=null) ? student.Attendances.ToList() : new();
            var logbookList = new List<LogbookEntry>();
            attendanceList = attendanceList.Where(a => a.LogbookEntry != null).OrderByDescending(a => a.AttendanceId).ToList();    
            attendanceList.ForEach(a => logbookList.Add(a.LogbookEntry!));
            if (status != null) logbookList = logbookList.Where(l => l.LogbookStatus == status).ToList();
            if (startDate != null) logbookList = logbookList.Where(l => DateOnly.FromDateTime(UtcDateTimeHelper.FromUtcToLocal(l.CreationTimestamp!.Value)) >= startDate).ToList();
            if (endDate != null) logbookList = logbookList.Where(l => DateOnly.FromDateTime(UtcDateTimeHelper.FromUtcToLocal(l.CreationTimestamp!.Value)) <= endDate).ToList();
            var logbookDtoList = new List<LogbookDto>();
            logbookList.ForEach(l => logbookDtoList.Add(_mapper.Map<LogbookDto>(l)));
            return (logbookDtoList, null);
        }

        public async Task<(List<LogbookDto>?, ErrorResponseModel?)> GetLogbooksByMentorWithFilteringAsync(int mentorId, LogbookStatus? status, DateOnly? startDate, DateOnly? endDate)
        {
            var key = "mentor";
            var mentor = await _mentorRepo.GetMentorByIdAsync(mentorId, true, true);
            if (mentor == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, mentorId.ToString())));

            var students = mentor.Students!.ToList();
            var attendanceList = new List<Attendance>();

            students.ForEach(s => attendanceList.AddRange(s.Attendances!.ToList()));

            var logbookList = new List<LogbookEntry>();
            attendanceList = attendanceList.Where(a => a.LogbookEntry != null).OrderByDescending(a => a.AttendanceId).ToList();
            attendanceList.ForEach(a => logbookList.Add(a.LogbookEntry!));
            if (status != null) logbookList = logbookList.Where(l => l.LogbookStatus == status).ToList();
            if (startDate != null) logbookList = logbookList.Where(l => DateOnly.FromDateTime(UtcDateTimeHelper.FromUtcToLocal(l.CreationTimestamp!.Value)) >= startDate).ToList();
            if (endDate != null) logbookList = logbookList.Where(l => DateOnly.FromDateTime(UtcDateTimeHelper.FromUtcToLocal(l.CreationTimestamp!.Value)) <= endDate).ToList();
            var logbookDtoList = new List<LogbookDto>();
            logbookList.ForEach(l => logbookDtoList.Add(_mapper.Map<LogbookDto>(l)));
            return (logbookDtoList, null);
        }
    }
}

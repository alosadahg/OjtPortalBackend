﻿using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IAttendanceService
    {
        Task<(Attendance?, ErrorResponseModel?)> TimeInAsync(int id);
        Task<(Attendance?, ErrorResponseModel?)> GetAttendanceById(int id);
        Task<(Attendance?, ErrorResponseModel?)> TimeOutAsync(int id);
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly IStudentRepo _studentRepo;
        private readonly IAttendanceRepo _attendanceRepo;
        private readonly IHolidayService _holidayService;

        public AttendanceService(IStudentRepo studentRepo, IAttendanceRepo attendanceRepo, IHolidayService holidayService)
        {
            this._studentRepo = studentRepo;
            this._attendanceRepo = attendanceRepo;
            this._holidayService = holidayService;
        }

        public async Task<(Attendance?, ErrorResponseModel?)> TimeInAsync(int id)
        {
            var student = await _studentRepo.GetStudentByIdAsync(id, false, false, true);
            if (student == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", id.ToString())));
            var attendance = new Attendance
            {
                Student = student,
                TimeIn = DateTime.UtcNow,
            };
            if (student.Shift == null) return (null, new(HttpStatusCode.BadRequest, LoggingTemplate.MissingRecordTitle("shift"), LoggingTemplate.MissingRecordDescription("shift", id.ToString())));
            var dateToday = DateOnly.FromDateTime(DateTime.Now);
            var recentAttendance = _attendanceRepo.GetRecentAttendance(student);
            
            if (recentAttendance != null)
            {
                recentAttendance!.TimeIn = TimeZoneInfo.ConvertTimeFromUtc(recentAttendance.TimeIn, TimeZoneInfo.Local);
                if (recentAttendance!.TimeOut != null) recentAttendance!.TimeOut = TimeZoneInfo.ConvertTimeFromUtc(attendance.TimeOut.Value, TimeZoneInfo.Local);
                if (DateOnly.FromDateTime(recentAttendance!.TimeIn).Equals(dateToday)) return (null, new(HttpStatusCode.Conflict, "Time in already recorded", "Today's time in is already recorded."));
                if (recentAttendance.TimeOut == null) return (null, new(HttpStatusCode.Conflict, "Recent attendance not yet clocked out", "Has not clocked out yet from previous attendance, please clock out."));
            }

            if (student.InternshipStatus.Equals(InternshipStatus.Pending)) await _studentRepo.UpdateStudentInternshipStatus(student, InternshipStatus.Ongoing);
            await _attendanceRepo.AddAttendanceAsync(attendance);
            return (attendance, null);
        }

        public async Task<(Attendance?, ErrorResponseModel?)> GetAttendanceById(int id)
        {
            var attendance = await _attendanceRepo.GetAttendanceByIdAsync(id);
            if (attendance == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("attendance"), LoggingTemplate.MissingRecordDescription("attendance", id.ToString())));
            return (attendance, null);
        }

        public async Task<(Attendance?, ErrorResponseModel?)> TimeOutAsync(int id)
        {
            var student = await _studentRepo.GetStudentByIdAsync(id, false, false, true);
            if (student == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", id.ToString())));
            if (student.Shift == null) return (null, new(HttpStatusCode.BadRequest, LoggingTemplate.MissingRecordTitle("shift"), LoggingTemplate.MissingRecordDescription("shift", id.ToString())));
           
            var recentAttendance = _attendanceRepo.GetRecentAttendance(student);
            if (recentAttendance == null || recentAttendance!.TimeOut != null) return (null, new(HttpStatusCode.Conflict, "No recent clock in", "Has not clocked in yet, please clock in first."));

            recentAttendance = await _attendanceRepo.TimeOutAsync(recentAttendance);
            return (recentAttendance, null);
        }
    }
}

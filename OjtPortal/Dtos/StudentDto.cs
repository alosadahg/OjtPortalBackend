﻿using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class StudentDto : UserDto
    {
        public int Id { get; set; } = 0;
        public string StudentId { get; set; } = string.Empty;
        public int DegreeProgramId { get; set; } = 0;
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public int MentorId { get; set; } = 0;
        public int InstructorId { get; set; } = 0;
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new();
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; } = InternshipStatus.Pending;
        public ShiftRecordDto ShiftRecord { get; set; } = new();

        public StudentDto()
        {
        }
    }

    public class NewStudentDto : NewUserDto
    {
        public string StudentId { get; set; } = string.Empty;
        public int DegreeProgramId { get; set; } = 0;
        public int MentorId { get; set; } = 0;
        public int InstructorId { get; set; } = 1;
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int HrsToRender { get; set; } = 0;
        public int ShiftRecordId { get; set; } = 0;
    }
}

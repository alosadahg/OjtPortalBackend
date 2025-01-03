﻿using OjtPortal.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class TeacherDto 
    {
        public ExistingUserDto User { get; set; } = new();
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public int StudentCount { get; set; } = 0;
        [JsonIgnore]
        public IEnumerable<StudentToInstructorOverviewDto>? Students { get; set; }
        public TeacherDto()
        {
            this.StudentCount = (Students != null) ? Students.Count() : 0;
        }
    }

    public class TeacherNoStudentsDto
    {
        public ExistingUserDto User { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
    }

    public class TeacherDtoWithStudents
    {
        public IEnumerable<StudentToInstructorOverviewDto>? Students { get; set; }
        public ExistingUserDto User { get; set; } = new();
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public int StudentCount { get; set; } = 0;
        
        public TeacherDtoWithStudents()
        {
            this.StudentCount = (Students != null) ? Students.Count() : 0;
        }
    }

    public class NewTeacherDto : UserDto
    {
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Department code is required")]
        public string DepartmentCode { get; set; } = string.Empty;
        [JsonIgnore]
        public Department Department { get; set; } = new();
        [Required(ErrorMessage = "Designation is required")]
        public string Designation { get; set; } = string.Empty;
    }

}

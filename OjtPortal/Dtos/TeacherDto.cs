using OjtPortal.Entities;
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
        public IEnumerable<StudentToInstructorOverviewDto>? Interns { get; set; }
        public TeacherDto()
        {
            this.StudentCount = (Interns != null) ? Interns.Count() : 0;
        }
    }

    public class NewTeacherDto : NewUserDto
    {
        public string DepartmentCode { get; set; } = string.Empty;
        [JsonIgnore]
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
    }

}

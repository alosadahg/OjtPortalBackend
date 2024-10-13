
using OjtPortal.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class MentorDto 
    {
        public ExistingUserDto User { get; set; } = new();
        public Company Company { get; set; } = new();
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
    }

    public class NewMentorDto : NewUserDto
    {
        [Required(ErrorMessage = "Company information is required")]
        public NewCompanyDto Company { get; set; } = new();
        [Required(ErrorMessage = "Mentor Department is required")]
        public string Department { get; set; } = string.Empty;
        [Required(ErrorMessage = "Mentor Designation is required")]
        public string Designation { get; set; } = string.Empty;
    }

    public class FullMentorDto : MentorDto
    {
        public int InternCount { get; set; } = 0;
        [JsonIgnore]
        public IEnumerable<StudentToMentorOverviewDto>? Interns { get; set; }
        public FullMentorDto()
        {
            this.InternCount = (Interns != null) ? Interns.Count() : 0;
        }

    }

    public class FullMentorDtoWithStudents : MentorDto
    {
        public IEnumerable<StudentToMentorOverviewDto>? Interns { get; set; }
        public int InternCount { get; set; } = 0;
        public FullMentorDtoWithStudents()
        {
            this.InternCount = (Interns != null) ? Interns.Count() : 0;
        }

    }

}

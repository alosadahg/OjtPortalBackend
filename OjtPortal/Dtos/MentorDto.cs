
using OjtPortal.Entities;
using OjtPortal.Services;
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
        [JsonIgnore]
        public IEnumerable<StudentToMentorOverviewDto>? Interns { get; set; }
        [JsonIgnore]
        public IEnumerable<SubMentor>? SubMentors { get; set; }
        public int SubMentorCount => SubMentors?.Count() ?? 0;
        public int InternCount => Interns?.Count() ?? 0;

    }

    public class FullMentorDtoWithStudents : MentorDto
    {
        public IEnumerable<StudentToMentorOverviewDto>? Interns { get; set; }
        public int InternCount => Interns?.Count() ?? 0;
    }

    public class FullMentorDtoWithSubMentors : MentorDto
    {
        public List<MentorDto>? SubMentors { get; set; }
        public int SubMentorCount => SubMentors?.Count() ?? 0;
    }
}

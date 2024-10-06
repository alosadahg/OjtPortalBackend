
using OjtPortal.Entities;
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
        public NewCompanyDto Company { get; set; } = new();
        public string Department { get; set; } = string.Empty;
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
}

using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class CompanyWithMentorsDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
        public List<MentorWithoutCompanyDto>? Mentors { get; set; }
    }

    public class CompanyWithFullMentorsDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
        public List<FullMentorWithoutCompanyDto>? Mentors { get; set; }
    }
}

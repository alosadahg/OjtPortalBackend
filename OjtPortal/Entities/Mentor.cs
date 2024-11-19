using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class Mentor 
    {
        [Key]
        [Column("MentorId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Company Company { get; set; } = new();
        public int CompanyId { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        [JsonIgnore]
        public IEnumerable<Student>? Students { get; set; }
        [JsonIgnore]
        public IEnumerable<SubMentor>? SubMentors { get; set; }

        public Mentor()
        {
        }
    }
}

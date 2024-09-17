using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class Teacher
    {
        [Key]
        [Column("TeacherId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        [JsonIgnore]
        public IEnumerable<Student>? Students { get; set; }
        public Teacher()
        {
        }

    }
}

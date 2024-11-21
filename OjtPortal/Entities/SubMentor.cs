using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Entities
{
    public class SubMentor
    {
        [Key]
        [Column("SubmentorId")]
        public int SubmentorId { get; set; }
        [ForeignKey("SubmentorId")]
        public Mentor? Submentor { get; set; }
        public int HeadMentorId { get; set; }
        public Mentor HeadMentor { get; set; } = new();
    }
}

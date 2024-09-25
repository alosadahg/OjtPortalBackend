using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Entities
{
    public class Chair 
    {
        [Key]
        [Column("ChairId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;


    }
}

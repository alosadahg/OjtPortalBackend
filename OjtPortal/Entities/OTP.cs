using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Entities
{
    public class OTP
    {
        [Key]
        [Column("Id")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; } = new();
        public string Code { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class CreatedUserDto
    {
        public User? User { get; set; }
        public bool IsPasswordGenerated { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}

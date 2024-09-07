using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Entities
{
    public class Holiday
    {
        [Key] 
        public string Uuid { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateOnly Date { get; set; }
        public string Type { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

    }
}

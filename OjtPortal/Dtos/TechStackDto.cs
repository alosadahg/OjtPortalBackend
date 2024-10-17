using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class TechStackDto
    {
    }

    public class NewTechStackDto
    {
        [Required(ErrorMessage = "Tech stack name is required if there if tech stacks list is not null")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tech stack type is required if there if tech stacks list is not null")]
        public string Type { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tech stack description is required if there if tech stacks list is not null")]
        public string Description { get; set; } = string.Empty;
    }
}

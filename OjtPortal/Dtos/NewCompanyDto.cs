using OjtPortal.Entities;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class NewCompanyDto
    {
        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Company contact number is required")]
        public string ContactNo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Company contact email is required")]
        public string ContactEmail { get; set; } = string.Empty;
        [Required(ErrorMessage = "Company address is required")]
        public Address Address { get; set; } = new();
    }
}

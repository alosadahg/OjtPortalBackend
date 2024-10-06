using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Reset password code is required")]
        public string Code { get; set; } = string.Empty;
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangeDefaultPasswordDto
    {
        public int Id { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

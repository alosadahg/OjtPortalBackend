using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OjtPortal.Dtos;
using OjtPortal.Services;

namespace OjtPortal.Pages
{
    public class ChangeDefaultPassword : PageModel
    {
		private readonly IUserService _userService;

		[BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;
        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

		public ChangeDefaultPassword(IUserService userService)
		{
			this._userService = userService;
		}

        public void OnGet()
        {
            // Clear TempData when the page is accessed via GET
            TempData.Remove("ErrorMessage");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TempData.Remove("ErrorMessage");
            if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                TempData["ErrorMessage"] = "Password must not be empty";
                return Page();
            }
            if (!NewPassword.Equals(ConfirmPassword))
            {
                TempData["ErrorMessage"] = "Passwords do not match";
                return Page();
            }

            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = Id,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var (message, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto);

            if(error != null)
            {
                TempData["Message"] = error.Errors.First().Message;
            }

            return RedirectToPage("RedirectChangeDefaultPassword");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using OjtPortal.Services;
using System.Text.Json;

namespace OjtPortal.Pages
{
    public class ChangeDefaultPassword : PageModel
    {
        private readonly IUserService _userService;
        private readonly IDepartmentRepo _departmentRepo;
        private readonly IDegreeProgramRepo _degreeProgramRepo;
        private readonly ITeacherRepo _teacherRepo;

        // Properties bound to the form fields
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public bool PendingStudentUpdate { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool PendingEmailUpdate { get; set; }

        [BindProperty]
        public string StudentId { get; set; } = string.Empty;
        [BindProperty]
        public string Email { get; set; } = string.Empty;
        [BindProperty]
        public string FirstName { get; set; } = string.Empty;
        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        public int? Department { get; set; } = null;
        [BindProperty]
        public int? DegreeProgram { get; set; } = null;
        [BindProperty]
        public int? Instructor { get; set; } = null;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;
        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<Department> Departments { get; set; } = new();
        public List<DegreeProgram> DegreePrograms { get; set; } = new();
        public List<Teacher>? Teachers { get; set; } = new();

        public ChangeDefaultPassword(
            IUserService userService,
            IDepartmentRepo departmentRepo,
            IDegreeProgramRepo degreeProgramRepo,
            ITeacherRepo teacherRepo)
        {
            _userService = userService;
            _departmentRepo = departmentRepo;
            _degreeProgramRepo = degreeProgramRepo;
            _teacherRepo = teacherRepo;
        }

        public async Task OnGetAsync()
        {
            Departments = await _departmentRepo.GetDepartmentsAsync();
            DegreePrograms = await _degreeProgramRepo.GetDegreeProgramsAsync();
            Teachers = await _teacherRepo.GetTeachersAsync();

            TempData.Remove("ErrorMessage");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Departments = await _departmentRepo.GetDepartmentsAsync();
            DegreePrograms = await _degreeProgramRepo.GetDegreeProgramsAsync();
            Teachers = await _teacherRepo.GetTeachersAsync();

            if (Email != null && !EmailChecker.IsEmailValid(Email))
            {
                TempData["ErrorMessage"] = "Please use your institutional email";
                ModelState.AddModelError(string.Empty, "Invalid email format.");
                return Page();
            }

            if (!NewPassword.Equals(ConfirmPassword))
            {
                TempData["ErrorMessage"] = "Passwords do not match";
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = Id,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var (message, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto, Token);

            if (error != null)
            {
                TempData["Message"] = error.Errors.First().Message;
                return Page();
            }

            return RedirectToPage("RedirectChangeDefaultPassword");
        }

        public async Task<IActionResult> OnPostDepartmentSelectAsync()
        {
            DegreePrograms = await _degreeProgramRepo.GetDegreeProgramsAsync();
            DegreePrograms = DegreePrograms.Where(dp => dp.DepartmentId == Department).ToList();
            Teachers = await _teacherRepo.GetTeachersAsync();
            Teachers = Teachers!.Where(t => t.Department.DepartmentId == Department).ToList();

            return new JsonResult(new
            {
                DegreePrograms,
                Teachers
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
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
        private readonly IStudentService _studentService;

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
            ITeacherRepo teacherRepo,
            IStudentService studentService)
        {
            _userService = userService;
            _departmentRepo = departmentRepo;
            _degreeProgramRepo = degreeProgramRepo;
            _teacherRepo = teacherRepo;
            _studentService = studentService;
        }

        public async Task OnGetAsync()
        {
            Departments = await _departmentRepo.GetDepartmentsAsync();

            TempData.Remove("ErrorMessage");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DegreePrograms = await _degreeProgramRepo.GetDegreeProgramsAsync();
            DegreePrograms = DegreePrograms.Where(dp => dp.DepartmentId == Department).ToList();
            Teachers = await _teacherRepo.GetTeachersAsync();
            Teachers = Teachers!.Where(t => t.Department.DepartmentId == Department).ToList();

            Departments = await _departmentRepo.GetDepartmentsAsync();

            if (!string.IsNullOrEmpty(Email) && !EmailChecker.IsEmailValid(Email))
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

            if(PendingStudentUpdate)
            {
                var updatedStudentInfo = new UpdateStudentDto
                {
                    User = new(Id, UserType.Student, AccountStatus.Pending, Email!, FirstName, LastName),
                    StudentId = StudentId,
                    DegreeProgram = DegreePrograms.Where(dp => dp.Id == DegreeProgram).First(),
                    InstructorId = Instructor
                };
                var (_, updateError) = await _studentService.UpdateStudentInfoAsync(updatedStudentInfo);
                if(updateError!= null)
                {
                    TempData["ErrorMessage"] = updateError.Errors.First().Message;
                    return Page();
                }
            }

            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = Id,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var (message, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto, Token, !PendingEmailUpdate);

            if (error != null)
            {
                TempData["ErrorMessage"] = error.Errors.First().Message;
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

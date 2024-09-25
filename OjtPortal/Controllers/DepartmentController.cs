using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentController : OjtPortalBaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            this._departmentService = departmentService;
        }

        /// <summary>
        /// Gets all college departments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDepartmentsAsync()
        {
            var (departments, error) = await _departmentService.GetDepartmentsAsync();
            if (error != null) return MakeErrorResponse(error);
            return Ok(departments);
        }
    }
}

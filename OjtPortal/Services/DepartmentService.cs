using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IDepartmentService
    {
        Task<(List<Department>?, ErrorResponseModel?)> GetDepartmentsAsync();
        Task<(Department?, ErrorResponseModel?)> GetByDepartmentCodeAsync(string code);
        Task<(Department?, ErrorResponseModel?)> GetByIdAsync(int id);
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly ICacheService _cacheService;
        private readonly IDepartmentRepo _departmentRepository;
        private readonly ILogger _logger;
        private static List<Department> _departments = new();

        public DepartmentService(ICacheService cacheService, IDepartmentRepo departmentRepository, ILogger<DepartmentService> logger)
        {
            this._cacheService = cacheService;
            this._departmentRepository = departmentRepository;
            this._logger = logger;
        }

        public async Task<(List<Department>?, ErrorResponseModel?)> GetDepartmentsAsync()
        {
            var key = "departments";
            if (_departments.Any())
            {
                _logger.LogInformation($"Returning existing local {key}.");
                return (_departments, null);
            }
            _departments = _cacheService.GetFromPermanentCache<List<Department>>(key) ?? new();
            if (!_departments.Any())
            {
                _logger.LogInformation(LoggingTemplate.CacheMissProceedToDatabase(key));
                _departments = await _departmentRepository.GetDepartmentsAsync();
                _cacheService.AddToPermanentCache(key, _departments);
            }
            if (_departments == null) return (null, new(HttpStatusCode.NotFound, $"Missing {key}", $"No {key} found"));
            return (_departments, null);
        }

        public async Task<(Department?, ErrorResponseModel?)> GetByDepartmentCodeAsync(string code)
        {
            if (!_departments.Any()) await GetDepartmentsAsync();
            var department = _departments.FirstOrDefault(department => department.DepartmentCode == code);
            if (department == null) return (null, new(HttpStatusCode.NotFound,
                LoggingTemplate.MissingRecordTitle("department"), LoggingTemplate.MissingRecordDescription("department", code)));
            return (department, null);
        }

        public async Task<(Department?, ErrorResponseModel?)> GetByIdAsync(int id)
        {
            if (!_departments.Any()) await GetDepartmentsAsync();
            var department = _departments.FirstOrDefault(d => d.DepartmentId == id);
            if (department == null) return (null, new(HttpStatusCode.NotFound,
                LoggingTemplate.MissingRecordTitle("department"), LoggingTemplate.MissingRecordDescription("department", id.ToString())));
            return (department, null);
        }
    }
}

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
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly ICacheService _cacheService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger _logger;
        private static List<Department> _departments = new();

        public DepartmentService(ICacheService cacheService, IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
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

    }
}

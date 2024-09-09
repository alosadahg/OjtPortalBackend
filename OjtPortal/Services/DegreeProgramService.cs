using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IDegreeProgramService
    {
        Task<(List<DegreeProgramDto>?, ErrorResponseModel?)> GetDegreeProgramsAsync();
    }

    public class DegreeProgramService : IDegreeProgramService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<DegreeProgramService> _logger;
        private readonly IDegreeProgramRepository _degreeProgramRepository;
        private readonly IMapper _mapper;
        private static List<DegreeProgramDto> _degreePrograms = new();

        public DegreeProgramService(ICacheService cacheService, ILogger<DegreeProgramService> logger, IDegreeProgramRepository degreeProgramRepository, IMapper mapper)
        {
            this._cacheService = cacheService;
            this._logger = logger;
            this._degreeProgramRepository = degreeProgramRepository;
            this._mapper = mapper;
        }

        public async Task<(List<DegreeProgramDto>?, ErrorResponseModel?)> GetDegreeProgramsAsync()
        {
            var key = "degree programs";
            if (_degreePrograms.Any())
            {
                _logger.LogInformation($"Returning existing local {key}.");
                return (_degreePrograms, null);
            }
            var degreePrograms = _cacheService.GetFromPermanentCache<List<DegreeProgram>>(key) ?? new();
            if (!degreePrograms.Any())
            {
                _logger.LogInformation(LoggingTemplate.CacheMissProceedToDatabase(key));
                degreePrograms = await _degreeProgramRepository.GetDegreeProgramsAsync();
                _cacheService.AddToPermanentCache(key, degreePrograms);
            }
            if (degreePrograms == null) return (null, new(HttpStatusCode.NotFound, $"Missing {key}", $"No {key} found"));
            degreePrograms.ForEach(dg =>
            {
                var dgDto = _mapper.Map<DegreeProgramDto>(dg);
                dgDto.DepartmentCode = dg.Department.DepartmentCode;
                _degreePrograms.Add(dgDto);
            });
            return (_degreePrograms, null);

        }

        // TODO: Add Get degree programs by given department (Priority: Normal)
    }
}

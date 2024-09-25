using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface ITeacherService
    {
        Task<(TeacherDto?, ErrorResponseModel?)> AddNewTeacherAsync(NewTeacherDto newTeacherDto);
        Task<(TeacherDto?, ErrorResponseModel?)> GetTeacherByIdAsync(int id);
    }

    public class TeacherService : ITeacherService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITeacherRepo _teacherRepository;
        private readonly IDepartmentService _departmentService;

        public TeacherService(IUserService userService, IMapper mapper, ITeacherRepo teacherRepository, IDepartmentService departmentService)
        {
            this._userService = userService;
            this._mapper = mapper;
            this._teacherRepository = teacherRepository;
            this._departmentService = departmentService;
        }

        public async Task<(TeacherDto?, ErrorResponseModel?)> AddNewTeacherAsync(NewTeacherDto newTeacherDto)
        {
            var (department, departmentError) = await _departmentService.GetByDepartmentCode(newTeacherDto.DepartmentCode);
            if (departmentError != null) return (null, departmentError);
            newTeacherDto.Department = department!;

            newTeacherDto.Password = _userService.GeneratePassword();
            var (createdUser, userError) = await _userService.CreateUserAsync(newTeacherDto, UserType.Teacher);
            if (userError != null) return (null, userError);

            var teacher = _mapper.Map<Teacher>(newTeacherDto);
            teacher.User = createdUser;
            var added = await _teacherRepository.AddTeacherAsync(teacher);

            if (added == null)
            {
                return (null, new(HttpStatusCode.NotFound,
                                  LoggingTemplate.DuplicateRecordTitle("teacher"),
                                  LoggingTemplate.DuplicateRecordDescription("teacher", newTeacherDto.Email)));
            }
            return (_mapper.Map<TeacherDto>(added), null);
        }

        public async Task<(TeacherDto?, ErrorResponseModel?)> GetTeacherByIdAsync(int id)
        {
            var existingTeacher = await _teacherRepository.GetTeacherByIdAsync(id);
            if (existingTeacher == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("teacher"), LoggingTemplate.MissingRecordDescription("teacher", $"{id}")));
            return (_mapper.Map<TeacherDto>(existingTeacher), null);
        }
    }
}

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
        Task<(TeacherDto?, ErrorResponseModel?)> GetTeacherByIdAsync(int id, bool includeUser);
        Task<List<TeacherDto>?> GetTeacherByDepartmentAsync(Department department);
        Task<List<TeacherDto>?> FindTeachersByDepartmentCode(DepartmentCode departmentName);
        Task<(List<TeacherDto>?, ErrorResponseModel?)> GetTeacherByDepartmentIdAsync(int departmentId);
        Task<(StudentDto?, ErrorResponseModel?)> TeacherAddStudentAsync(TeacherAddStudentDto studentDto);
    }

    public class TeacherService : ITeacherService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITeacherRepo _teacherRepository;
        private readonly IDepartmentService _departmentService;
        private readonly IStudentService _studentService;
        private readonly IUserRepo _userRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IDegreeProgramRepo _degreeProgramRepo;

        public TeacherService(IUserService userService, IMapper mapper, ITeacherRepo teacherRepository, IDepartmentService departmentService, IStudentService studentService, IUserRepo userRepo, IStudentRepo studentRepo, IDegreeProgramRepo degreeProgramRepo)
        {
            this._userService = userService;
            this._mapper = mapper;
            this._teacherRepository = teacherRepository;
            this._departmentService = departmentService;
            this._studentService = studentService;
            this._userRepo = userRepo;
            this._studentRepo = studentRepo;
            this._degreeProgramRepo = degreeProgramRepo;
        }

        public async Task<(TeacherDto?, ErrorResponseModel?)> AddNewTeacherAsync(NewTeacherDto newTeacherDto)
        {
            var (department, departmentError) = await _departmentService.GetByDepartmentCodeAsync(newTeacherDto.DepartmentCode);
            if (departmentError != null) return (null, departmentError);
            newTeacherDto.Department = department!;

            var (createdUser, userError) = await _userService.CreateUserAsync(newTeacherDto, newTeacherDto.Password, UserType.Teacher);
            if (userError != null) return (null, userError);

            var teacher = _mapper.Map<Teacher>(newTeacherDto);
            teacher.User = createdUser!.User;
            var added = await _teacherRepository.AddTeacherAsync(teacher);

            if (added == null)
            {
                return (null, new(HttpStatusCode.NotFound,
                                  LoggingTemplate.DuplicateRecordTitle("_teacherService"),
                                  LoggingTemplate.DuplicateRecordDescription("_teacherService", newTeacherDto.Email)));
            }

            if (createdUser!.IsPasswordGenerated)
            {
                newTeacherDto.Password = createdUser.Password;
                var emailError = _userService.SendActivationEmailAsync(newTeacherDto.Email, createdUser.User!, newTeacherDto.Password);
                if (emailError.Result != null) return (null, emailError.Result);
            }
            else
            {
                var emailError = _userService.SendActivationEmailAsync(newTeacherDto.Email, createdUser.User!);
                if (emailError.Result != null) return (null, emailError.Result);
            }
            return (_mapper.Map<TeacherDto>(added), null);
        }

        public async Task<(TeacherDto?, ErrorResponseModel?)> GetTeacherByIdAsync(int id, bool includeUser)
        {
            var existingTeacher = await _teacherRepository.GetTeacherByIdAsync(id, includeUser);
            if (existingTeacher == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("_teacherService"), LoggingTemplate.MissingRecordDescription("_teacherService", $"{id}")));

            var teacherDto = _mapper.Map<TeacherDto>(existingTeacher);
            teacherDto.Students = _studentService.MapStudentsToDtoList<StudentToInstructorOverviewDto>(existingTeacher.Students!);
            teacherDto.StudentCount = teacherDto.Students.Count();
            return (teacherDto, null);
        }

        public async Task<List<TeacherDto>?> FindTeachersByDepartmentCode(DepartmentCode departmentName)
        {
            int departmentId = (int)departmentName + 1;
            var (department, error) = await _departmentService.GetByIdAsync(departmentId);

            return await GetTeacherByDepartmentAsync(department!);
        }

        public async Task<List<TeacherDto>?> GetTeacherByDepartmentAsync(Department department)
        {
            var existingTeachers = await _teacherRepository.GetTeacherByDepartmentAsync(department) ?? new();
            List<TeacherDto> teachers = new();
            existingTeachers.ForEach(t =>
            {
                var teacherDto = _mapper.Map<TeacherDto>(t);
                teacherDto.Students = _studentService.MapStudentsToDtoList<StudentToInstructorOverviewDto>(t.Students!);
                teacherDto.StudentCount = teacherDto.Students.Count();
                teachers.Add(teacherDto);
            });
            return teachers;
        }

        public async Task<(List<TeacherDto>?, ErrorResponseModel?)> GetTeacherByDepartmentIdAsync(int departmentId)
        {
            var (department, error) = await _departmentService.GetByIdAsync(departmentId);
            if (error != null) return (null, error);
            return (await GetTeacherByDepartmentAsync(department!), null);
        }

        public async Task<(StudentDto?, ErrorResponseModel?)> TeacherAddStudentAsync(TeacherAddStudentDto studentToAddDto)
        {
            var studentEntity = _mapper.Map<Student>(studentToAddDto);
            var key = "teacher";

            var existingTeacher = await _teacherRepository.GetTeacherByIdAsync(studentToAddDto.InstructorId!.Value, true);
            if (existingTeacher == null) return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, studentToAddDto.InstructorId!.ToString()))));
            studentEntity.Instructor = existingTeacher;

            var (existingStudentUser, _) = await _userRepo.GetUserByEmailAsync(studentToAddDto.Email);

            if (existingStudentUser == null)
            {
                var newStudentDto = _mapper.Map<NewStudentDto>(studentToAddDto);
                var (studentDto, studentRegisterError) = await _studentService.RegisterStudentAsync(newStudentDto, true);
                if (studentRegisterError != null) return (null, studentRegisterError);
                return (studentDto, null);
            }

            key = "degree program";
            studentEntity.DegreeProgram = await _degreeProgramRepo.FindDegreeProgramById(studentToAddDto.DegreeProgramId!.Value);
            if (existingTeacher == null) return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, studentToAddDto.InstructorId!.ToString()))));
            studentEntity = await _studentRepo.UpdateStudentByTeacherAsync(studentEntity, existingStudentUser.Id);

            return (_mapper.Map<StudentDto>(studentEntity), null);
        }
    }
}

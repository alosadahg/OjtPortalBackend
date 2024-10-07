using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.EmailTemplates;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface IStudentService
    {
        Task<(DateOnly?, ErrorResponseModel?)> GetEndDateAsync(DateOnly? startDate, int manDays, bool includeHolidays, WorkingDays workingDays);
        Task<(StudentDto?, ErrorResponseModel?)> RegisterStudentAsync(NewStudentDto newStudent, bool withEmailChecking);
        Task<(StudentDto?, ErrorResponseModel?)> RegisterStudentAsync(NewStudentDto newStudent);
        Task<(StudentDto?, ErrorResponseModel?)> GetStudentByIdAsync(int id, bool includeUser);
        List<T> MapStudentsToDtoList<T>(IEnumerable<Student> students) where T : class;
        Task<(StudentDto?, ErrorResponseModel?)> UpdateStudentInfoAsync(UpdateStudentDto updateStudentDto);
        int CalculateManDays(int hrs, int dailyHours);
    }

    public class StudentService : IStudentService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHolidayService _holidayService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly ITeacherRepo _teacherRepository;
        private readonly IMentorRepo _mentorRepository;
        private readonly ILogger<StudentService> _logger;
        private readonly IDegreeProgramRepo _degreeProgramRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IUserRepo _userRepo;

        public StudentService(UserManager<User> userManager, IHolidayService holidayService, IMapper mapper, IEmailSender emailSender, IUserService userService, ITeacherRepo teacherRepository, IMentorRepo mentorRepository, ILogger<StudentService> logger, IDegreeProgramRepo degreeProgramRepo, IStudentRepo studentRepo, IUserRepo userRepo)
        {
            this._userManager = userManager;
            this._holidayService = holidayService;
            this._mapper = mapper;
            this._emailSender = emailSender;
            this._userService = userService;
            this._teacherRepository = teacherRepository;
            this._mentorRepository = mentorRepository;
            this._logger = logger;
            this._degreeProgramRepo = degreeProgramRepo;
            this._studentRepo = studentRepo;
            this._userRepo = userRepo;
        }

        public async Task<(StudentDto?, ErrorResponseModel?)> RegisterStudentAsync(NewStudentDto newStudent, bool withEmailChecking)
        {
            if(withEmailChecking && !EmailChecker.IsEmailValid(newStudent.Email)) return (null, new(HttpStatusCode.BadRequest, "Invalid email", "Please use your institutional email (ends with @cit.edu or @noopmail.org if testing)"));
            return await RegisterStudentAsync(newStudent);
        }
        public async Task<(StudentDto?, ErrorResponseModel?)> RegisterStudentAsync(NewStudentDto newStudent)
        {
            var key = "degree program";
            var existingProgram = (newStudent.DegreeProgramId.HasValue) ? await _degreeProgramRepo.FindDegreeProgramById(newStudent.DegreeProgramId!.Value) : null;
            // if (existingProgram == null) return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, newStudent.DegreeProgramId.ToString()))));

            var studentEntity = _mapper.Map<Student>(newStudent);
            if (await _studentRepo.IsStudentExistingAsync(studentEntity)) return (null, new(HttpStatusCode.BadRequest, LoggingTemplate.DuplicateRecordTitle("student"), LoggingTemplate.DuplicateRecordDescription("student", newStudent.StudentId)));

            key = "mentor";
            if (newStudent.MentorId.HasValue)
            {
                var existingMentor = await _mentorRepository.GetMentorByIdAsync(newStudent.MentorId.Value, true);
                studentEntity.Mentor = existingMentor;
            }
            //if(existingMentor == null) return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, newStudent.MentorId.ToString()))));

            key = "teacher";
            if (newStudent.TeacherId.HasValue)
            {
                var existingTeacher = await _teacherRepository.GetTeacherByIdAsync(newStudent.TeacherId.Value, true);
                studentEntity.Instructor = existingTeacher;
            }
             //if (existingTeacher == null) return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle(key), LoggingTemplate.MissingRecordDescription(key, newStudent.TeacherId.ToString()))));

            key = "student";
            var (createdUser, error) = await _userService.CreateUserAsync(newStudent, newStudent.Password, UserType.Student);
            if(error != null) return (null, error);

            // Linking the student to its object fields
            if(existingProgram!= null) studentEntity.DegreeProgram = existingProgram;
            studentEntity.User = createdUser!.User;
            
            if(existingProgram != null) existingProgram.Department.Students!.Add(studentEntity);

            if (studentEntity.StartDate != null && studentEntity.HrsToRender > 0 && studentEntity.Shift!=null)
            {
                studentEntity.ManDays = CalculateManDays(studentEntity.HrsToRender, studentEntity.Shift.DailyDutyHrs);
                var (endDate, dateError) = await GetEndDateAsync(studentEntity.StartDate, studentEntity.ManDays, studentEntity.Shift.IncludePublicPhHolidays, studentEntity.Shift.WorkingDays);
                if (dateError != null) return (null, dateError);
                studentEntity.EndDate = endDate!.Value;
            }

            studentEntity = await _studentRepo.AddStudentAsync(studentEntity);
            if (studentEntity == null) return (null, new(HttpStatusCode.UnprocessableEntity, LoggingTemplate.DuplicateRecordTitle(key), LoggingTemplate.DuplicateRecordDescription(key, newStudent.Email)));

            if (createdUser!.IsPasswordGenerated)
            {
                newStudent.Password = createdUser.Password;
                var emailError = _userService.SendActivationEmailAsync(newStudent.Email, studentEntity.User!, newStudent.Password);
                if (emailError.Result != null) return (null, emailError.Result);
            }
            else
            {
                var emailError = _userService.SendActivationEmailAsync(newStudent.Email, studentEntity.User!);
                if (emailError.Result != null) return (null, emailError.Result);
            }

            return (_mapper.Map<StudentDto>(studentEntity), null);
        }

        public async Task<(StudentDto?, ErrorResponseModel?)> GetStudentByIdAsync(int id, bool includeUser)
        {
            var student = await _studentRepo.GetStudentByIdAsync(id, includeUser);
            if(student == null)
            {
                return (null, new(HttpStatusCode.NotFound, new ErrorModel(LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", id.ToString()))));
            }
            return (_mapper.Map<StudentDto>(student), null);
        }

        public async Task<(DateOnly?, ErrorResponseModel?)> GetEndDateAsync(DateOnly? startDate, int manDays, bool includeHolidays, WorkingDays workingDays)
        {
            var (holidays, error) = await _holidayService.GetHolidaysAsync();
            if (error != null) return (null, error);
            var holidayDates = holidays!.Select(h => h.Date).ToList();
            if (!startDate.HasValue) return (null, new(HttpStatusCode.BadRequest, "Missing Start Date", "Start Date is null"));
            DateOnly endDate = startDate!.Value;

            while (manDays > 0)
            {
                bool flagSkip = false;

                flagSkip = (holidayDates.Contains(endDate) && !includeHolidays) ? true : false;
                if (endDate.DayOfWeek == DayOfWeek.Saturday && workingDays.Equals(WorkingDays.WeekdaysOnly)) flagSkip = true;
                else if (endDate.DayOfWeek == DayOfWeek.Sunday && !workingDays.Equals(WorkingDays.WholeWeek)) flagSkip = true;

                if (!flagSkip)
                {
                    manDays--;
                }
                if (manDays > 0) endDate = endDate.AddDays(1);
            }

            return (endDate, null);
        }

        public List<T> MapStudentsToDtoList<T>(IEnumerable<Student> students) where T : class
        {
            var list = new List<T>();
            if (students != null) {
                foreach (var student in students)
                {
                    list.Add(_mapper.Map<T>(student));
                }
            }
            return list;
        }

        public int CalculateManDays(int hrs, int dailyHours)
        {
            return (int) Math.Ceiling((hrs / (double) dailyHours));
        }

        public async Task<(StudentDto?, ErrorResponseModel?)> UpdateStudentInfoAsync(UpdateStudentDto updateStudentDto)
        {
            var student = _mapper.Map<Student>(updateStudentDto);
            student = await _studentRepo.UpdateStudentInfoAsync(student);
            if (student == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("student"), LoggingTemplate.MissingRecordDescription("student", updateStudentDto.User!.Id.ToString())));
            if (!string.IsNullOrEmpty(updateStudentDto.User!.Email)) await  _userService.ResendActivationEmailAsync(updateStudentDto.User.Email);
            
            return (_mapper.Map<StudentDto>(student), null);
        }
    }
}
                                            
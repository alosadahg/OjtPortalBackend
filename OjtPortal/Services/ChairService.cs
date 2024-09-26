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
    public interface IChairService
    {
        Task<(ChairDto?, ErrorResponseModel?)> AddNewChairAsync(NewTeacherDto newChairDto);
        Task<(ChairDto?, ErrorResponseModel?)> GetChairByIdAsync(int id, bool includeUser);
    }

    public class ChairService : IChairService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IChairRepo _chairRepository;
        private readonly IDepartmentService _departmentService;
        private readonly ITeacherService _teacherService;

        public ChairService(IUserService userService, IMapper mapper, IChairRepo ChairRepository, IDepartmentService departmentService, ITeacherService teacherService)
        {
            this._userService = userService;
            this._mapper = mapper;
            this._chairRepository = ChairRepository;
            this._departmentService = departmentService;
            this._teacherService = teacherService;
        }

        public async Task<(ChairDto?, ErrorResponseModel?)> AddNewChairAsync(NewTeacherDto newChairDto)
        {
            var (department, departmentError) = await _departmentService.GetByDepartmentCodeAsync(newChairDto.DepartmentCode);
            if (departmentError != null) return (null, departmentError);
            newChairDto.Department = department!;

            newChairDto.Password = _userService.GenerateToken("password");
            var (createdUser, userError) = await _userService.CreateUserAsync(newChairDto, newChairDto.Password, UserType.Chair);
            if (userError != null) return (null, userError);

            var chair = _mapper.Map<Chair>(newChairDto);
            chair.User = createdUser!.User;
            var added = await _chairRepository.AddChairAsync(chair);

            if (added == null)
            {
                return (null, new(HttpStatusCode.NotFound,
                                  LoggingTemplate.DuplicateRecordTitle("chair"),
                                  LoggingTemplate.DuplicateRecordDescription("chair", newChairDto.Email)));
            }

            if (createdUser!.IsPasswordGenerated)
            {
                newChairDto.Password = createdUser.Password;
                var emailError = _userService.SendActivationEmailAsync(newChairDto.Email, createdUser.User!, newChairDto.Password);
                if (emailError.Result != null) return (null, emailError.Result);
            }
            else
            {
                var emailError = _userService.SendActivationEmailAsync(newChairDto.Email, createdUser.User!);
                if (emailError.Result != null) return (null, emailError.Result);
            }

            var chairDto = _mapper.Map<ChairDto>(added);
            var teachers = await _teacherService.GetTeacherByDepartmentAsync(department!);
            chairDto.Teachers = teachers;
            if (teachers != null)
            {
                teachers.ForEach(t =>
                {
                    chairDto.TeacherCount += 1;
                    chairDto.StudentCount += t.StudentCount;
                });
            }
            return (chairDto, null);
        }

        public async Task<(ChairDto?, ErrorResponseModel?)> GetChairByIdAsync(int id, bool includeUser)
        {
            var existingChair = await _chairRepository.GetChairByIdAsync(id, true);
            if (existingChair == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("chair"), LoggingTemplate.MissingRecordDescription("chair", $"{id}")));
            var chairDto = _mapper.Map<ChairDto>(existingChair);
            var teachers = await _teacherService.GetTeacherByDepartmentAsync(existingChair.Department!);
            chairDto.Teachers = teachers;
            if (teachers != null)
            {
                teachers.ForEach(t =>
                {
                    chairDto.TeacherCount += 1;
                    chairDto.StudentCount += t.StudentCount;
                });
            }
            return (chairDto, null);
        }
    }
}

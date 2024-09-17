using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.EmailTemplates;

namespace OjtPortal.Services
{
    public interface IStudentService
    {
        Task<(DateOnly?, ErrorResponseModel?)> GetEndDateAsync(DateOnly startDate, int manDays, bool includeHolidays, WorkingDays workingDays);
        Task<(StudentDto?, ErrorResponseModel?)> RegisterStudent(NewStudentDto newStudent);
    }

    public class StudentService : IStudentService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHolidayService _holidayService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;

        public StudentService(UserManager<User> userManager, IHolidayService holidayService, IMapper mapper, IEmailSender emailSender, IUserService userService)
        {
            this._userManager = userManager;
            this._holidayService = holidayService;
            this._mapper = mapper;
            this._emailSender = emailSender;
            this._userService = userService;
        }

        public async Task<(StudentDto?, ErrorResponseModel?)> RegisterStudent(NewStudentDto newStudent)
        {
            var (newUser, error) = await _userService.CreateUserAsync(newStudent, UserType.Student);
            if (error != null) return (null, error);

            return (null, null);
            //TODO: Add registration logic for student (Priority: Urgent)

        }

        public async Task<(DateOnly?, ErrorResponseModel?)> GetEndDateAsync(DateOnly startDate, int manDays, bool includeHolidays, WorkingDays workingDays)
        {
            var (holidays, error) = await _holidayService.GetHolidaysAsync();
            if (error != null) return (null, error);
            var holidayDates = holidays!.Select(h => h.Date).ToList();
            DateOnly endDate = startDate;

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
    }
}

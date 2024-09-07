using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;

namespace OjtPortal.Services
{
    public interface IStudentService
    {
        Task<(DateOnly?, ErrorResponseModel?)> GetEndDate(DateOnly startDate, int manDays, bool includeHolidays, WorkingDays workingDays);
        void RegisterStudent(NewStudentDto newStudent);
    }

    public class StudentService : IStudentService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHolidayService _holidayService;
        private readonly IMapper _mapper;

        public StudentService(UserManager<User> userManager, IHolidayService holidayService, IMapper mapper)
        {
            this._userManager = userManager;
            this._holidayService = holidayService;
            this._mapper = mapper;
        }

        public void RegisterStudent(NewStudentDto newStudent)
        {
            var newStudentEntity = _mapper.Map<Student>(newStudent);
            Console.WriteLine("eyy");
        }

        public async Task<(DateOnly?, ErrorResponseModel?)> GetEndDate(DateOnly startDate, int manDays, bool includeHolidays, WorkingDays workingDays)
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

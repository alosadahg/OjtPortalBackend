using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;

namespace OjtPortal.Repositories
{
    public interface IHolidayRepo
    {
        Task<Holiday?> AddHolidayAsync(Holiday newHoliday);
        Task<List<Holiday>?> AddAllHolidaysAsync(List<Holiday> newHolidays);
        Task<List<Holiday>?> GetHolidaysAsync(int year);
    }

    public class HolidayRepo : IHolidayRepo
    {
        private readonly OjtPortalContext _context;

        public HolidayRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Holiday?> AddHolidayAsync(Holiday newHoliday)
        {
            var existingHoliday = await _context.Holidays.FindAsync(newHoliday.Uuid);
            if (existingHoliday != null) return null;
            await _context.Holidays.AddAsync(newHoliday);
            _context.SaveChanges();
            return newHoliday;
        }

        public async Task<List<Holiday>?> AddAllHolidaysAsync(List<Holiday> newHolidays)
        {
            var existingHolidays = await _context.Holidays.ToListAsync();
            _context.Holidays.RemoveRange(existingHolidays);
            await _context.Holidays.AddRangeAsync(newHolidays);
            _context.SaveChanges();
            return newHolidays;
        }

        public async Task<List<Holiday>?> GetHolidaysAsync(int year)
        {
            return await _context.Holidays.Where(h => h.Date.Year == year).ToListAsync();
        }
    }
}

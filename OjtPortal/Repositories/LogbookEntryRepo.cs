using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;
using System.Diagnostics.Eventing.Reader;

namespace OjtPortal.Repositories
{
    public interface ILogbookEntryRepo
    {
        Task<LogbookEntry> AddLogbookEntryAsync(LogbookEntry logbookEntry);
        Task<LogbookEntry?> GetLogbookByIdAsync(long logbookId);
    }

    public class LogbookEntryRepo : ILogbookEntryRepo
    {
        private readonly OjtPortalContext _context;

        public LogbookEntryRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<LogbookEntry> AddLogbookEntryAsync(LogbookEntry logbookEntry)
        {
            _context.LogbookEntries.Add(logbookEntry);
            await _context.SaveChangesAsync();
            return logbookEntry;
        }

        public async Task<LogbookEntry?> GetLogbookByIdAsync(long logbookId)
        {
            return await _context.LogbookEntries.Include(l => l.Attendance).FirstOrDefaultAsync(l => l.AttendanceId == logbookId);
        }
    }
}

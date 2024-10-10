using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ILogbookEntryRepo
    {
        Task<LogbookEntry> AddLogbookEntryAsync(LogbookEntry logbookEntry);
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
    }
}

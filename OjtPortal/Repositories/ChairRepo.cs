using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IChairRepo
    {
        Task<Chair?> AddChairAsync(Chair newChair);
        Task<Chair?> GetChairByIdAsync(int id, bool includeUser);
        Task<bool> IsChairExisting(Chair chair);
    }

    public class ChairRepo : IChairRepo
    {
        private readonly OjtPortalContext _context;

        public ChairRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Chair?> AddChairAsync(Chair newChair)
        {
            if (IsChairExisting(newChair).Result) return null;
            _context.Entry(newChair.Department).State = EntityState.Unchanged;
            await _context.Chairs.AddAsync(newChair);
            await _context.SaveChangesAsync();
            return newChair;
        }

        public async Task<bool> IsChairExisting(Chair chair)
        {
            return await _context.Chairs.ContainsAsync(chair);
        }

        public async Task<Chair?> GetChairByIdAsync(int id, bool includeUser)
        {
            var query = (includeUser) ? await _context.Chairs.Include(c => c.User).Include(c => c.Department).FirstOrDefaultAsync(c => c.UserId == id)
                : await _context.Chairs.Include(c => c.Department).FirstOrDefaultAsync(c => c.UserId == id);
            return query;
        }
    }
}

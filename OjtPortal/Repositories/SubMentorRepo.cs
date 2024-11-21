using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ISubMentorRepo
    {
        Task<SubMentor?> AddSubMentorAsync(SubMentor subMentor);
        Task<bool> IsSubMentorExisting(SubMentor subMentor);
    }

    public class SubMentorRepo : ISubMentorRepo
    {
        private readonly OjtPortalContext _context;
        private readonly ILogger<SubMentorRepo> _logger;

        public SubMentorRepo(OjtPortalContext context, ILogger<SubMentorRepo> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<SubMentor?> AddSubMentorAsync(SubMentor subMentor)
        {
            if (await IsSubMentorExisting(subMentor)) return subMentor;
            try
            {
                _context.Entry(subMentor).State = EntityState.Unchanged;
                _context.Entry(subMentor.HeadMentor).State = EntityState.Unchanged;

                _context.SubMentors.Add(subMentor);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
            return subMentor;
        }

        public async Task<bool> IsSubMentorExisting(SubMentor subMentor)
        {
            var existing = await _context.SubMentors.AnyAsync(sb => sb.SubmentorId.Equals(subMentor.SubmentorId) && sb.HeadMentorId.Equals(subMentor.HeadMentor.UserId));
            return existing;
        }
    }
}

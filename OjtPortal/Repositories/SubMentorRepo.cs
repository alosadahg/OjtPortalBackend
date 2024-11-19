using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public class SubMentorRepo
    {
        private readonly OjtPortalContext _context;

        public SubMentorRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<SubMentor?> AddSubMentorAsync(SubMentor subMentor)
        {
            if (IsSubMentorExisting(subMentor).Result) return subMentor;
            try
            {
                _context.SubMentors.Add(subMentor);
                await _context.SaveChangesAsync();
            } catch (Exception)
            {
                return null;
            }
            return subMentor;
        }

        public async Task<bool> IsSubMentorExisting(SubMentor subMentor)
        {
            var existing = await _context.SubMentors.FirstOrDefaultAsync(sb => sb.UserId.Equals(subMentor.UserId) && sb.HeadMentorId.Equals(subMentor.HeadMentorId));
            return (existing == null) ? false : true ;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IMentorRepo
    {
        Task<Mentor?> AddMentorAsync(Mentor newMentor);
        Task<bool> IsMentorExisting(Mentor mentor);
        Task<Mentor?> GetMentorByIdAsync(int id, bool includeStudents, bool includeStudentsAttendance, bool includeSubmentors);
    }

    public class MentorRepo : IMentorRepo
    {
        private readonly OjtPortalContext _context;

        public MentorRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<Mentor?> AddMentorAsync(Mentor newMentor)
        {
            if (IsMentorExisting(newMentor).Result) return null;
            await _context.Mentors.AddAsync(newMentor);
            await _context.SaveChangesAsync();
            return newMentor;
        }

        public async Task<bool> IsMentorExisting(Mentor mentor)
        {
            return await _context.Mentors.ContainsAsync(mentor);
        }

        public async Task<Mentor?> GetMentorByIdAsync(int id, bool includeStudents, bool includeStudentsAttendance, bool includeSubMentors = false)
        {
            IQueryable<Mentor> query = _context.Mentors.Include(m => m.User).Include(m => m.Company);
            if (includeStudents && includeStudentsAttendance)
                query = query.Include(m => m.Students)!.ThenInclude(s => s.Attendances)!.ThenInclude(a => a.LogbookEntry);
            else if (includeStudents)
            {
                query = query.Include(m => m.Students)!.ThenInclude(s => s.User);
                query = query.Include(m => m.Students)!.ThenInclude(s => s.Instructor).ThenInclude(i => i.User);
            }
            if(includeSubMentors)
            {
                query = query.Include(m => m.SubMentors)!.ThenInclude(sb => sb.Submentor);
            }
            return await query.FirstOrDefaultAsync(m => m.UserId == id);
        }
    }
}

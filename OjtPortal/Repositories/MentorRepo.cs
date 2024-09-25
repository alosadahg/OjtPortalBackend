﻿using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface IMentorRepo
    {
        Task<Mentor?> AddMentorAsync(Mentor newMentor);
        Task<bool> IsMentorExisting(Mentor mentor);
        Task<Mentor?> GetMentorByIdAsync(int id, bool includeUser);
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

        public async Task<Mentor?> GetMentorByIdAsync(int id, bool includeUser)
        {
            var query = (includeUser) ? await _context.Mentors.Include(m => m.User).Include(m => m.Company).Include(m => m.Students).FirstOrDefaultAsync(m => m.UserId == id)
                : await _context.Mentors.Include(m => m.Company).Include(m => m.Students).FirstOrDefaultAsync(m => m.UserId == id);
            return query;
        }
    }
}

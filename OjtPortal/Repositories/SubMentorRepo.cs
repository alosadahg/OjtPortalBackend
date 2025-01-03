﻿using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ISubMentorRepo
    {
        Task<SubMentor?> AddSubMentorAsync(SubMentor subMentor);
        Task<bool> IsSubMentorExisting(SubMentor subMentor);
        Task<SubMentor?> IsRecordExisting(int headMentorId, int subMentorId);
        Task<Mentor> TransferMentorshipToSubmentorAsync(List<TrainingPlan>? trainingPlans, List<Student>? students, List<SubMentor>? subMentors, SubMentor submentor);
        Task<bool> HasHeadMentorAsync(int submentorId);
        Task<SubMentor?> GetSubMentorByIdAsync(int id, bool includeTasks);
        Task<SubMentor> AssignTaskToSubmentorAsync(SubMentor subMentor, TrainingTask task);

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

        public async Task<SubMentor?> IsRecordExisting(int headMentorId, int subMentorId)
        {
            var existing = await _context.SubMentors.FirstOrDefaultAsync(sb => sb.HeadMentorId.Equals(headMentorId) && sb.SubmentorId.Equals(subMentorId));
            if(existing != null) return existing;
            existing = await _context.SubMentors.FirstOrDefaultAsync(sb => sb.HeadMentorId.Equals(subMentorId) && sb.SubmentorId.Equals(headMentorId));
            return existing;
        }

        public async Task<bool> HasHeadMentorAsync(int submentorId)
        {
            return await _context.SubMentors.Where(sm => sm.SubmentorId == submentorId).AnyAsync();
        }

        public async Task<SubMentor?> GetSubMentorByIdAsync(int id, bool includeTasks)
        {
            IQueryable<SubMentor> query = _context.SubMentors;
            if(includeTasks) query = query.Include(sm => sm.TrainingTask);
            return await query.FirstOrDefaultAsync(sm => sm.SubmentorId == id);
        }

        public async Task<SubMentor> AssignTaskToSubmentorAsync(SubMentor subMentor, TrainingTask task)
        {
            var taskList = subMentor.TrainingTask!.ToList();
            if (!taskList.Contains(task))
            {
                taskList.Add(task);
                subMentor.TrainingTask = taskList;
            }
            await _context.SaveChangesAsync();
            return subMentor;
        }

        public async Task<Mentor> TransferMentorshipToSubmentorAsync(List<TrainingPlan>? trainingPlans, List<Student>? students, List<SubMentor>? subMentors, SubMentor submentor)
        {
            if(students != null) 
            students.ForEach(student =>
            {
                student.Mentor = submentor.Submentor;
            });
            if(trainingPlans != null)
            trainingPlans.ForEach(trainingPlan =>
            {
                trainingPlan.Mentor = submentor.Submentor;
            });
            _context.SubMentors.Remove(submentor);
            if(subMentors != null)
            subMentors.ForEach(sb =>
            {
                sb.HeadMentor = submentor.Submentor!;
            });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return submentor.Submentor!;
        }
    }
}

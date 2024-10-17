using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;

namespace OjtPortal.Repositories
{
    public interface ITaskRepo
    {
        Task<List<TrainingTask>> GetSyntheticTasksAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter);
    }

    public class TaskRepo : ITaskRepo
    {
        private readonly OjtPortalContext _context;

        public TaskRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<List<TrainingTask>> GetSyntheticTasksAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter)
        {
            var tasks = await _context.TrainingTasks.Include(t => t.Skills).Include(t => t.TechStacks).Where(t => t.IsSystemGenerated).ToListAsync();
            if (titleFilter != null) tasks = tasks.Where(t => t.Title.ToLower().Contains(titleFilter.ToLower())).ToList();
            if (descriptionFilter != null) tasks = tasks.Where(t => t.Description.ToLower().Contains(descriptionFilter.ToLower())).ToList();
            if (difficulty != null) tasks = tasks.Where(t => t.Difficulty == difficulty).ToList();
            if (techStackFilter != null)
                tasks = tasks.Where(t => t.TechStacks.Any(s => s.Name.ToLower().Contains(techStackFilter.ToLower()))).ToList();
            if (skillFilter != null)
                tasks = tasks.Where(t => t.Skills.Any(s => s.Name.ToLower().Contains(skillFilter.ToLower()))).ToList();
            return tasks;
        }
    }
}

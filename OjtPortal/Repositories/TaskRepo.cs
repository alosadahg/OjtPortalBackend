using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Repositories
{
    public interface ITaskRepo
    {
        Task<List<TrainingTask>> GetSyntheticTasksAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter);
        Task<TrainingPlan?> AddTaskToPlanAsync(TrainingPlan trainingPlan, TrainingTask trainingTask);
        Task<TrainingTask?> GetTaskByIdAsync(int id);
        Task<TrainingTask> UpdateTaskAsync(TrainingTask previousTask, TrainingTask updatedTask);
    }

    public class TaskRepo : ITaskRepo
    {
        private readonly OjtPortalContext _context;
        private readonly ILogger<TaskRepo> _logger;

        public TaskRepo(OjtPortalContext context, ILogger<TaskRepo> logger)
        {
            this._context = context;
            this._logger = logger;
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

        public async Task<TrainingPlan?> AddTaskToPlanAsync(TrainingPlan trainingPlan, TrainingTask trainingTask)
        {
            try
            {
                trainingPlan.Tasks.Add(trainingTask);
                var studentTasks = await _context.StudentTasks
                    .Include(st => st.StudentTraining)
                    .Where(st => st.StudentTraining!.TrainingPlanId == trainingTask.TrainingPlanId)
                    .ToListAsync();

                var uniqueStudentIds = studentTasks.Select(st => st.StudentTrainingId).Distinct().ToList();
                foreach (var id in uniqueStudentIds)
                {
                    var studentTask = new StudentTask
                    {
                        StudentTrainingId = id,
                        TrainingTask = trainingTask,
                        DueDate = null
                    };
                    await _context.StudentTasks.AddAsync(studentTask);
                }
                await _context.SaveChangesAsync();
                return trainingPlan;
            } catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task<TrainingTask?> GetTaskByIdAsync(int id)
        {
            return await _context.TrainingTasks.Include(t => t.TrainingPlan).Include(t => t.TechStacks).Include(t => t.Skills).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TrainingTask> UpdateTaskAsync(TrainingTask previousTask, TrainingTask updatedTask)
        {
            _context.Skills.RemoveRange(previousTask.Skills);
            _context.TechStacks.RemoveRange(previousTask.TechStacks);
            previousTask.Skills = updatedTask.Skills;
            previousTask.Difficulty = updatedTask.Difficulty;
            previousTask.Description = updatedTask.Description;
            previousTask.TechStacks = updatedTask.TechStacks;
            previousTask.Title = updatedTask.Title;
            try
            {
                await _context.SaveChangesAsync();
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return previousTask;
        } 
    }
}

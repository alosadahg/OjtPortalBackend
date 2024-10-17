using Microsoft.EntityFrameworkCore;
using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ITrainingPlanRepo
    {
        Task<TrainingPlan> AddTrainingPlanAsync(TrainingPlan trainingPlan);
        Task<TrainingPlan?> FetchExistingSystemGeneratedTrainingPlanAsync(TrainingPlan trainingPlan);
        Task<TrainingPlan?> CheckSystemGeneratedTrainingPlanAsync(string position, string division, int totalHrs, int dailyDutyHrs);
        Task<TrainingPlan?> GetTrainingPlanByIdAsync(int id);
    }

    public class TrainingPlanRepo : ITrainingPlanRepo
    {
        private readonly OjtPortalContext _context;
        private readonly ILogger<TrainingPlanRepo> _logger;

        public TrainingPlanRepo(OjtPortalContext context, ILogger<TrainingPlanRepo> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<TrainingPlan> AddTrainingPlanAsync(TrainingPlan trainingPlan)
        {
            var existingPlan = await FetchExistingSystemGeneratedTrainingPlanAsync(trainingPlan);
            if (existingPlan != null)
            {
                if (trainingPlan.MentorId != null && trainingPlan.MentorId == existingPlan.MentorId) return existingPlan;
                if (trainingPlan.MentorId == null) return existingPlan;
            }
            _context.TrainingPlans.Add(trainingPlan);
            await _context.SaveChangesAsync();
            return trainingPlan;
        }

        public async Task<TrainingPlan?> FetchExistingSystemGeneratedTrainingPlanAsync(TrainingPlan trainingPlan)
        {
            return await _context.TrainingPlans.Where(tp => tp.Title == trainingPlan.Title && tp.Description == trainingPlan.Description && tp.TotalTasks == tp.TotalTasks && tp.EasyTasksCount == trainingPlan.EasyTasksCount && tp.MediumTasksCount == trainingPlan.MediumTasksCount && tp.HardTasksCount == trainingPlan.HardTasksCount).FirstOrDefaultAsync();
        }

        public async Task<TrainingPlan?> CheckSystemGeneratedTrainingPlanAsync(string position, string division, int totalHrs, int dailyDutyHrs)
        {
            position = position.Replace("/", " or ");
            position = position.Replace("&", " and ");

            division = division.Replace("/", " or ");
            division = division.Replace("&", " and ");
            _logger.LogInformation($"Position: {position} | Division: {division} | TotalHrs: {totalHrs} | DailyDutyHrs: {dailyDutyHrs}");
            var existingPlans = await _context.TrainingPlans
                .Where(tp => tp.Description.ToLower().Contains(position.ToLower()) &&
                             tp.Description.ToLower().Contains(totalHrs.ToString()) &&
                             tp.Description.ToLower().Contains(division.ToLower()))
                .ToListAsync();
            foreach (var existing in existingPlans) {
                var totalTasks = (int)Math.Floor((double)totalHrs / (dailyDutyHrs * 5));
                if (existing != null)
                {
                    _logger.LogInformation($"Existing: {existing.TotalTasks} | Total: {totalTasks}");
                    if (existing.MentorId != null) continue;
                    if (existing.TotalTasks == totalTasks) return existing;
                }
            }
            return null;
            
        }

        public async Task<TrainingPlan?> GetTrainingPlanByIdAsync(int id)
        {
            IQueryable<TrainingPlan> query = _context.TrainingPlans.Include(tp => tp.Tasks).ThenInclude(t => t.TechStacks);
            query = query.Include(tp => tp.Tasks).ThenInclude(t => t.Skills);
            return await query.FirstOrDefaultAsync(tp => tp.Id == id);
        }
    }
}

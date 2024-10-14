using OjtPortal.Context;
using OjtPortal.Entities;

namespace OjtPortal.Repositories
{
    public interface ITrainingPlanRepo
    {
        Task<TrainingPlan?> AddTrainingPlanAsync(TrainingPlan trainingPlan);
    }

    public class TrainingPlanRepo : ITrainingPlanRepo
    {
        private readonly OjtPortalContext _context;

        public TrainingPlanRepo(OjtPortalContext context)
        {
            this._context = context;
        }

        public async Task<TrainingPlan?> AddTrainingPlanAsync(TrainingPlan trainingPlan)
        {
            _context.TrainingPlans.Add(trainingPlan);
            await _context.SaveChangesAsync();
            return trainingPlan;
        }
    }
}

using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ITrainingPlanService
    {
        Task<(TrainingPlan?, ErrorResponseModel?)> AddTrainingPlanAsync(NewTrainingPlanDto newTrainingPlan);
    }

    public class TrainingPlanService : ITrainingPlanService
    {
        private readonly ITrainingPlanRepo _trainingPlanRepo;
        private readonly IMapper _mapper;

        public TrainingPlanService(ITrainingPlanRepo trainingPlanRepo, IMapper mapper)
        {
            this._trainingPlanRepo = trainingPlanRepo;
            this._mapper = mapper;
        }

        public async Task<(TrainingPlan?, ErrorResponseModel?)> AddTrainingPlanAsync(NewTrainingPlanDto newTrainingPlan)
        {
            var trainingPlan = _mapper.Map<TrainingPlan>(newTrainingPlan);
            trainingPlan = await _trainingPlanRepo.AddTrainingPlanAsync(trainingPlan);
            return (trainingPlan, null);
        }
    }
}

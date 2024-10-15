using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/training/plan")]
    public class TrainingPlanController : OjtPortalBaseController
    {
        private readonly ITrainingPlanService _trainingPlanService;

        public TrainingPlanController(ITrainingPlanService trainingPlanService)
        {
            this._trainingPlanService = trainingPlanService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTrainingPlanAsync(NewTrainingPlanDto newTrainingPlan)
        {
            var (result, error) = await _trainingPlanService.AddTrainingPlanAsync(newTrainingPlan);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

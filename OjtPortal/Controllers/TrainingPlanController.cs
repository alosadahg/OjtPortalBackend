using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Infrastructure;
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

        /*   [HttpPost]
           public async Task<IActionResult> AddTrainingPlanAsync(NewTrainingPlanDto newTrainingPlan)
           {
               var (result, error) = await _trainingPlanService.AddTrainingPlanAsync(newTrainingPlan);
               if (error != null) return MakeErrorResponse(error);
               return Ok(result);
           }*/

        /// <summary>
        /// Fetch and update the system generated training plans 
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrainingPlanDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPut("fetch/from/api")]
        public async Task<IActionResult> FetchTrainingPlanFromAPIAsync()
        {
            var (result, error) = await _trainingPlanService.AddSystemGeneratedTrainingPlanAsync();
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

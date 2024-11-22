using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Infrastructure;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/training/plans")]
    public class TrainingPlanController : OjtPortalBaseController
    {
        private readonly ITrainingPlanService _trainingPlanService;
        private readonly IStudentTrainingService _studentTrainingService;
        private readonly UserManager<User> _userManager;

        public TrainingPlanController(ITrainingPlanService trainingPlanService, IStudentTrainingService studentTrainingService, UserManager<User> userManager)
        {
            this._trainingPlanService = trainingPlanService;
            this._studentTrainingService = studentTrainingService;
            this._userManager = userManager;
        }

        /// <summary>
        /// Add a training plan with mentor id
        /// </summary>
        /// <param name="newTrainingPlan"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTrainingPlanAsync(NewTrainingPlanDto newTrainingPlan)
        {
            var (result, error) = await _trainingPlanService.AddTrainingPlanAsync(newTrainingPlan);
            if (error != null) return MakeErrorResponse(error);
            return CreatedAtRoute("GetTrainingPlanByIdAsync", new { id = result.Id }, result);
        }

        /// <summary>
        /// Fetch and update the system generated training plans 
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrainingPlanDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("fetch/from/api")]
        public async Task<IActionResult> FetchTrainingPlanFromAPIAsync()
        {
            var (result, error) = await _trainingPlanService.GetSystemGeneratedTrainingPlansAsync();
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Get training plan by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrainingPlanDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpGet("{id}", Name = "GetTrainingPlanByIdAsync")]
        public async Task<IActionResult> GetTrainingPlanByIdAsync(int id)
        {
            var (result, error) = await _trainingPlanService.GetTrainingPlanByIdAsync(id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Assign training plan to student (Requires auth)
        /// </summary>
        /// <param name="assignTrainingPlanDto"></param>
        /// <returns></returns>
        [HttpPut("assign")]
        [Authorize]
        public async Task<IActionResult> AssignTrainingPlanAsync(AssignTrainingPlanDto assignTrainingPlanDto)
        {
            var user = await _userManager.GetUserAsync(User);
            var (result, error) = await _studentTrainingService.AssignTrainingPlanAsync(assignTrainingPlanDto, user.Id);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Gets training plans by mentor 
        /// </summary>
        /// <param name="mentorId">The unique identifier of mentor</param>
        /// <returns></returns>
        [HttpGet("mentor/{mentorId}")]
        public async Task<IActionResult> GetTrainingPlansByMentorAsync(int mentorId)
        {
            var (result, error) = await _trainingPlanService.GetTrainingPlansByMentorAsync(mentorId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Gets training plan by student 
        /// </summary>
        /// <param name="studentUserId">The unique identifier of student</param>
        /// <returns></returns>
        [HttpGet("student/{studentUserId}")]
        public async Task<IActionResult> GetTrainingPlansByStudentAsync(int studentUserId)
        {
            var (result, error) = await _studentTrainingService.GetAssignedTrainingPlanToStudentAsync(studentUserId);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Update existing training plan
        /// </summary>
        /// <param name="updateTrainingPlanDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrainingPlanDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseModel))]
        [HttpPut]
        public async Task<IActionResult> UpdateTrainingPlanAsync(UpdateTrainingPlanDto updateTrainingPlanDto)
        {
            var (result, error) = await _trainingPlanService.UpdateTrainingPlanAsync(updateTrainingPlanDto);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

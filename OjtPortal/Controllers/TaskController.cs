using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OjtPortal.Controllers.BaseController.cs;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Services;

namespace OjtPortal.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : OjtPortalBaseController
    {
        private readonly ITaskService _taskService;
        private readonly IStudentTaskService _studentTaskService;
        private readonly UserManager<User> _userManager;

        public TaskController(ITaskService taskService, IStudentTaskService studentTaskService, UserManager<User> userManager)
        {
            this._taskService = taskService;
            this._studentTaskService = studentTaskService;
            this._userManager = userManager;
        }

        /// <summary>
        /// Get AI generated tasks with filtering
        /// </summary>
        /// <param name="titleFilter">Filters tasks by title</param>
        /// <param name="descriptionFilter">Filters tasks by description</param>
        /// <param name="difficulty">Filters tasks by difficulty</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<TaskDto>> GetSyntheticTasksWithFiltering(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty)
        {
            return await _taskService.GetSyntheticTasksWithFilteringAsync(titleFilter, descriptionFilter, difficulty, null, null);
        }

        /// <summary>
        /// Get AI generated tasks (include stack and skill) with filtering
        /// </summary>
        /// <param name="titleFilter">Filters tasks by title</param>
        /// <param name="descriptionFilter">Filters tasks by description</param>
        /// <param name="difficulty">Filters tasks by difficulty</param>
        /// <param name="techStackFilter">Filters tasks by tech stack name</param>
        /// <param name="skillFilter">Filters tasks by skill name</param>
        /// <returns></returns>
        [HttpGet("with/stacks/skills")]
        public async Task<List<TaskWithStackAndSkillDto>> GetSyntheticTasksWithStackAndSkillWithFiltering(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter)
        {
            return await _taskService.GetSyntheticFullTasksWithFilteringAsync(titleFilter, descriptionFilter, difficulty, techStackFilter, skillFilter);
        }

        /// <summary>
        /// Add new task to existing training plan
        /// </summary>
        /// <param name="addTaskToPlanDto"></param>
        /// <returns></returns>
        [HttpPut("add")]
        public async Task<IActionResult> AddTaskToPlanAsync(AddTaskToPlanDto addTaskToPlanDto)
        {
            var (result, error) = await _taskService.AddTaskToTrainingPlan(addTaskToPlanDto);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }

        /// <summary>
        /// Update task status
        /// </summary>
        /// /// <param name="userId">The student user id</param>
        /// <param name="taskId">Task id may be retrieved from getting the student training plan</param>
        /// <param name="updatedStatus">Updated task status</param>
        /// <returns></returns>
        [HttpPatch("{taskId}/user/{userId}")]
        public async Task<IActionResult> UpdateTaskStatusAsync(int userId,int taskId, TrainingTaskStatus updatedStatus)
        {
            var (result, error) = await _studentTaskService.UpdateTaskStatus(userId, taskId, updatedStatus);
            if (error != null) return MakeErrorResponse(error);
            return Ok(result);
        }
    }
}

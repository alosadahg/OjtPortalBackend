using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.EmailTemplates;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using System.Net;

namespace OjtPortal.Services
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetSyntheticTasksWithFilteringAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter);
        Task<List<TaskWithStackAndSkillDto>> GetSyntheticFullTasksWithFilteringAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter);
        Task<(TrainingPlan?, ErrorResponseModel?)> AddTaskToTrainingPlan(AddTaskToPlanDto addTaskToPlanDto);
        Task<TaskWithStackAndSkillDto?> GetTaskByIdAsync(int id);
    }

    public class TaskService : ITaskService
    {
        private readonly ITaskRepo _taskRepo;
        private readonly IMapper _mapper;
        private readonly ITrainingPlanRepo _trainingPlanRepo;
        private readonly IStudentTaskRepo _studentTaskRepo;

        public TaskService(ITaskRepo taskRepo, IMapper mapper, ITrainingPlanRepo trainingPlanRepo, IStudentTaskRepo studentTaskRepo)
        {
            this._taskRepo = taskRepo;
            this._mapper = mapper;
            this._trainingPlanRepo = trainingPlanRepo;
            this._studentTaskRepo = studentTaskRepo;
        }

        public async Task<List<TaskDto>> GetSyntheticTasksWithFilteringAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter)
        {
            var taskList = await _taskRepo.GetSyntheticTasksAsync(titleFilter, descriptionFilter, difficulty, techStackFilter, skillFilter);
            var taskDtoList = _mapper.Map<List<TaskDto>>(taskList);
            for (int i = 0; i < taskDtoList.Count; i++)
            {
                var originalTitle = taskDtoList[i].Title;

                if(originalTitle.Contains(":")) taskDtoList[i].Title = originalTitle.Substring(originalTitle.IndexOf(":") + 1).Trim();
            }
            return (taskDtoList);
        }

        public async Task<List<TaskWithStackAndSkillDto>> GetSyntheticFullTasksWithFilteringAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter)
        {
            var taskList = await _taskRepo.GetSyntheticTasksAsync(titleFilter, descriptionFilter, difficulty, techStackFilter, skillFilter);
            var taskDtoList = _mapper.Map<List<TaskWithStackAndSkillDto>>(taskList);
            for (int i = 0; i < taskDtoList.Count; i++)
            {
                var originalTitle = taskDtoList[i].Title;

                if (originalTitle.Contains(":")) taskDtoList[i].Title = originalTitle.Substring(originalTitle.IndexOf(":") + 1).Trim();
            }
            return (taskDtoList);
        }

        public async Task<(TrainingPlan?, ErrorResponseModel?)> AddTaskToTrainingPlan(AddTaskToPlanDto addTaskToPlanDto)
        {
            var trainingPlan = await _trainingPlanRepo.GetTrainingPlanByIdAsync(addTaskToPlanDto.TrainingPlanId);
            if (trainingPlan == null) return (null, new(HttpStatusCode.NotFound, LoggingTemplate.MissingRecordTitle("training plan"), LoggingTemplate.MissingRecordDescription("training plan", addTaskToPlanDto.TrainingPlanId.ToString())));
            if (addTaskToPlanDto.Difficulty.Equals(TaskDifficulty.Easy)) trainingPlan.EasyTasksCount++;
            if (addTaskToPlanDto.Difficulty.Equals(TaskDifficulty.Medium)) trainingPlan.MediumTasksCount++;
            if (addTaskToPlanDto.Difficulty.Equals(TaskDifficulty.Hard)) trainingPlan.HardTasksCount++;
            trainingPlan.TotalTasks++;
            var task = _mapper.Map<TrainingTask>(addTaskToPlanDto);
            task.TrainingPlanId = trainingPlan.Id;
            var updatedTrainingPlan = await _taskRepo.AddTaskToPlanAsync(trainingPlan, task);
            if (updatedTrainingPlan == null) return (null, new(HttpStatusCode.BadRequest, "Failed to add task", "An error occured while adding"));
            trainingPlan = updatedTrainingPlan;
            return (trainingPlan, null);
        }

        /*public async Task<(ErrorResponseModel?)> UpdateTaskAsync(UpdateTaskDto updateTaskDto)
        {
            var existingTask = _taskRepo
        }*/

        public async Task<TaskWithStackAndSkillDto?> GetTaskByIdAsync(int id)
        {
            var existing = await _taskRepo.GetTaskByIdAsync(id);
            if (existing == null) return null;
            return _mapper.Map<TaskWithStackAndSkillDto>(existing);
        }
    }
}

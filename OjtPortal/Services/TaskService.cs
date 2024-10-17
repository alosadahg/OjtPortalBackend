using AutoMapper;
using OjtPortal.Dtos;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;

namespace OjtPortal.Services
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetSyntheticTasksWithFilteringAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter);
        Task<List<TaskWithStackAndSkillDto>> GetSyntheticFullTasksWithFilteringAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter);
    }

    public class TaskService : ITaskService
    {
        private readonly ITaskRepo _taskRepo;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepo taskRepo, IMapper mapper)
        {
            this._taskRepo = taskRepo;
            this._mapper = mapper;
        }

        public async Task<List<TaskDto>> GetSyntheticTasksWithFilteringAsync(string? titleFilter, string? descriptionFilter, TaskDifficulty? difficulty, string? techStackFilter, string? skillFilter)
        {
            var taskList = await _taskRepo.GetSyntheticTasksAsync(titleFilter, descriptionFilter, difficulty, techStackFilter, skillFilter);
            var taskDtoList = _mapper.Map<List<TaskDto>>(taskList);
            for (int i = 0; i < taskDtoList.Count; i++)
            {
                var originalTitle = taskDtoList[i].Title;

                if(originalTitle.Contains(":")) taskDtoList[i].Title = originalTitle.Substring(originalTitle.IndexOf(":") + 1).Trim();

                taskDtoList[i].TechStackCount = taskList[i].TechStacks.Count;
                taskDtoList[i].SkillCount = taskList[i].Skills.Count;
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

                taskDtoList[i].TechStackCount = taskList[i].TechStacks.Count;
                taskDtoList[i].SkillCount = taskList[i].Skills.Count;
            }
            return (taskDtoList);
        }
    }
}

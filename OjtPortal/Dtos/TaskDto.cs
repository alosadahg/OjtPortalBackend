using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TaskDifficulty Difficulty { get; set; }
        public string Description { get; set; } = string.Empty;
        public int TechStackCount { get; set; } = 0;
        public int SkillCount { get; set; } = 0;
        public bool IsSystemGenerated { get; set; } = false;
    }

    public class TaskWithStackAndSkillDto : TaskDto
    {
        public List<TechStack> TechStacks { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
    }

    public class TaskFullDto : TaskDto
    {
        public TrainingPlanDto? TrainingPlan { get; set; }
    }

    public class NewTaskDto
    {
        [Required(ErrorMessage = "Task title is required")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Task description is required")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Task difficulty is required")]
        public TaskDifficulty Difficulty { get; set; }
        public List<NewTechStackDto> TechStacks { get; set; } = new();
        public List<NewSkillDto> Skills { get; set; } = new();
    }
}

using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TaskDifficulty Difficulty { get; set; }
        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual List<TechStack> TechStacks { get; set; } = new();

        [JsonIgnore]
        public virtual List<Skill> Skills { get; set; } = new();

        public int TechStackCount => TechStacks?.Count() ?? 0;
        public int SkillCount => Skills?.Count() ?? 0;

        public bool IsSystemGenerated { get; set; } = false;
    }

    public class TaskWithStackAndSkillDto : TaskDto
    {
        public override List<TechStack> TechStacks { get; set; } = new();
        public override List<Skill> Skills { get; set; } = new();
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

    public class AssignedTaskDto
    {
        public int Id { get; set; }
        public int StudentTrainingId { get; set; }
        public TrainingTask? TrainingTask { get; set; }
        public TrainingTaskStatus? TaskStatus { get; set; } = TrainingTaskStatus.NotStarted;
        public DateOnly? DueDate { get; set; } = new();
        public double Score { get; set; } = 0.0;
    }

    public class AddTaskToPlanDto : NewTaskDto
    {
        public int TrainingPlanId { get; set; }
    }

    public class UpdateTaskDto : NewTaskDto
    {
        public int TaskId { get; set; }
    }

    public class StudentTaskDto
    {
        public int Id { get; set; }
        public int StudentTrainingId { get; set; }
        public TrainingTask? TrainingTask { get; set; }
        public TrainingTaskStatus? TaskStatus { get; set; }
        public DateOnly? DueDate { get; set; } = new();
        public DateOnly? DateCompleted { get; set; } = new();
        public double Score { get; set; } = 0.0;
    }
}

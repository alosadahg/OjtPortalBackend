using OjtPortal.Entities;
using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class TaskDto
    {
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

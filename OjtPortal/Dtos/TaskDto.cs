using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class TaskDto
    {
    }

    public class NewTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<NewTechStackDto> TechStacks { get; set; } = new();
        public List<NewSkillDto> Skills { get; set; } = new();
    }
}

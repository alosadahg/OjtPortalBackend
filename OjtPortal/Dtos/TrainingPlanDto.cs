using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class TrainingPlanDto
    {
    }

    public class NewTrainingPlanDto
    {
        public int? MentorId { get; set; } = null;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<NewTaskDto> Tasks { get; set; } = new();
    }
}

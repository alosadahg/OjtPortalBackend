using Newtonsoft.Json;
using OjtPortal.Entities;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class TrainingPlanDto
    {
        public int Id { get; set; }
        public int? MentorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalTasks { get; set; } = 0;
        public int EasyTasksCount { get; set; } = 0;
        public int MediumTasksCount { get; set; } = 0;
        public int HardTasksCount { get; set; } = 0;
        public bool IsSystemGenerated { get; set; } = false;
    }

    public class NewTrainingPlanDto
    {
        [Required(ErrorMessage = "Mentor ID is required")]
        public int MentorId { get; set; }
        [Required(ErrorMessage = "Training plan title is required")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Training plan description is required")]
        public string Description { get; set; } = string.Empty;
        public List<NewTaskDto> Tasks { get; set; } = new();
    }

    public class TrainingPlanFromApiDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalTasks { get; set; } = 0;
        public int EasyTasksCount { get; set; } = 0;
        public int MediumTasksCount { get; set; } = 0;
        public int HardTasksCount { get; set; } = 0;
        public List<NewTaskDto> Tasks { get; set; } = new();
    }

    public class TrainingPlanApiResponse
    {
        [JsonProperty("training_plans")]
        public List<TrainingPlanFromApiDto> TrainingPlans { get; set; } = new();
    }

    public class TrainingPlanRequestDto
    {
        [JsonProperty("position")]
        public string Designation { get; set; } = string.Empty;
        [JsonProperty("division")]
        public string Division { get; set; } = string.Empty;
        [JsonProperty("total_hrs")]
        public int HrsToRender { get; set; } = 0;
        [JsonProperty("daily_duty_hrs")]
        public int DailyDutyHrs { get; set; } = 0;
    }
}

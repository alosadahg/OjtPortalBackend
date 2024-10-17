using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace OjtPortal.Dtos
{
    public class AssignTrainingPlanDto
    {
        [Required(ErrorMessage = "Training Plan ID is required")]
        public int TrainingPlanId { get; set; }
        [JsonProperty("studentUserId")]
        [Required(ErrorMessage ="Student User ID is required")]
        public int StudentId { get; set; }
        public List<AssignTaskWithDueDto>? TaskWithDueDtos { get; set; }
    }

    public class AssignTaskWithDueDto
    {
        public int? TaskId { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}

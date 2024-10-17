using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class StudentTraining
    {
        [Key]
        public int StudentId { get; set; }
        [JsonIgnore]
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
        public int TrainingPlanId { get; set; }
        [JsonIgnore]
        public TrainingPlan? TrainingPlan { get; set; }
        public List<StudentTask> Tasks { get; set; } = new();
        public int CompletedTaskCount { get; set; } = 0;
        [Column(TypeName = "varchar(50)")]
        public TrainingTaskStatus TrainingPlanStatus { get; set; } = TrainingTaskStatus.NotStarted;
        public int? DurationInHours { get; set; } = 0;
        public DateOnly ExpectedStartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly ExpectedEndDate { get; set; } = new();
    }
}

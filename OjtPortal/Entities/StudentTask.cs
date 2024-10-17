using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class StudentTask
    {
        public int Id { get; set; }
        public int StudentId {get; set;}
        [JsonIgnore]
        public Student? Student {get; set;}
        public int StudentTrainingId {get; set;}
        [JsonIgnore]
        public StudentTraining? StudentTraining {get; set;}
        public int TrainingTaskId { get; set;}
        [JsonIgnore]
        public TrainingTask? TrainingTask {get; set;}
        [Column(TypeName = "varchar(50)")]
        public TrainingTaskStatus? TaskStatus { get; set; } = TrainingTaskStatus.NotStarted;
        public int? TaskCompletionDurationInHrs { get; set; } = null;
        public DateOnly? DueDate { get; set; } = new();
        public double Score { get; set; } = 0.0;
    }
}

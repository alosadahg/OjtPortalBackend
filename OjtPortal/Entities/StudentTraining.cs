using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Entities
{
    public class StudentTraining
    {
        [Key]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
        public TrainingPlan? TrainingPlan { get; set; }
        public int CompletedTaskCount { get; set; } = 0;
        [Column(TypeName ="varchar(50)")]
        public TaskStatus TrainingPlanStatus { get; set; }
        public int DurationInHours { get; set; }
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new();
    }
}

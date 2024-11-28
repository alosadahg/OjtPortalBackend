using OjtPortal.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class TrainingTask
    {
        public int Id { get; set; }
        public int TrainingPlanId { get; set; }
        [JsonIgnore]
        public TrainingPlan? TrainingPlan { get; set; }
        public string Title { get; set; } = string.Empty;
        [Column(TypeName = "varchar(50)")]
        public TaskDifficulty Difficulty { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<TechStack> TechStacks { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
        public bool IsSystemGenerated { get; set; } = false;
        [JsonIgnore]
        public IEnumerable<SubMentor>? SubMentor { get; set; }
    }

}

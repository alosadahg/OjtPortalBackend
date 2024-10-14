using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class Skill
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [JsonIgnore]
        public List<TrainingTask>? Tasks { get; set; }
    }
}

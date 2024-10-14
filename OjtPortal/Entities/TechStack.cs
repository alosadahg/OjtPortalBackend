using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class TechStack
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Type {  get; set; } = string.Empty;
        [JsonIgnore]
        public List<TrainingTask>? Tasks { get; set; } = null;
    }
        
}

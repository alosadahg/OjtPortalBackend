using System.Security.Permissions;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    public class TrainingPlan
    {
		public int Id { get; set; }
		public int? MentorId { get; set; }
		[JsonIgnore]
		public Mentor? Mentor { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public List<TrainingTask> Tasks { get; set; } = new();
        public int TotalTasks { get; set; } = 0;
		public int EasyTasksCount { get; set; } = 0;
		public int MediumTasksCount { get; set; } = 0;
		public int HardTasksCount { get; set; } = 0;	
        public bool IsSystemGenerated { get; set; } = false;
    }
}

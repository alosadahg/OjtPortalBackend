namespace OjtPortal.Entities
{
    public class StudentTask
    {
        public int Id { get; set; }
        public int StudentId {get; set;}
        public Student? Student {get; set;}
        public int TrainingTaskId {get; set;}
        public TrainingTask? TrainingTask {get; set;}
        public TaskStatus? TaskStatus {get; set;}
        public int DurationInHours { get; set; } = new();
        public int? TaskCompletionDurationInHrs { get; set; } = null;
        public DateTime due = new();
    }
}

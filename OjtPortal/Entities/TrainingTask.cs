namespace OjtPortal.Entities
{
    public class TrainingTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<TechStack> TechStacks { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
    }

}

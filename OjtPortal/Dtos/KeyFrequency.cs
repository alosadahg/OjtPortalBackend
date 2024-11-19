namespace OjtPortal.Dtos
{
    public class KeyFrequency
    {
        public string Key { get; set; } = string.Empty;
        public int Usage { get; set; } = 0;
    }

    public class GroupKeyFrequency
    {
        public string GroupedBy { get; set; } = string.Empty;
        public List<KeyFrequency> Frequencies { get; set; } = new();
    }
}

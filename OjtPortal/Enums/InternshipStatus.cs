using System.ComponentModel;

namespace OjtPortal.Entities
{
    public enum InternshipStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Ongoing")]
        Ongoing,
        [Description("Completed")]
        Completed
    }
}

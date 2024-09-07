using System.ComponentModel;

namespace OjtPortal.Enums
{
    public enum AccountStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Active")]
        Active,
        [Description("Deactivated")]
        Deactivated
    }
}

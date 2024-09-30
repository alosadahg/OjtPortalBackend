using OjtPortal.Entities;
using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<AccountStatus>))]
    public enum AccountStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Pending Password Change")]
        PendingPasswordChange,
        [Description("Active")]
        Active,
        [Description("Deactivated")]
        Deactivated
    }
}

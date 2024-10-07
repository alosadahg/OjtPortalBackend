using OjtPortal.Entities;
using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<LogbookStatus>))]
    public enum LogbookStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Submitted")]
        Submitted
    }
}

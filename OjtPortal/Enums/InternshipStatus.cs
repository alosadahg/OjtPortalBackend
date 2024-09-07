using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    [JsonConverter(typeof(EnumDescriptionConverter<InternshipStatus>))]
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

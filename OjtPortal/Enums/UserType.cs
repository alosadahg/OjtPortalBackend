using OjtPortal.Entities;
using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Enums
{
    [JsonConverter(typeof(EnumDescriptionConverter<UserType>))]
    public enum UserType
    {
        [Description("Admin")]
        Admin,
        [Description("Student")]
        Student,
        [Description("Mentor")]
        Mentor,
        [Description("Teacher")]
        Teacher,
        [Description("Chair")]
        Chair
    }
}

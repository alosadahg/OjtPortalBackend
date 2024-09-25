using OjtPortal.Infrastructure.JsonConverters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OjtPortal.Entities
{
    [JsonConverter(typeof(EnumDescriptionConverter<DepartmentCode>))]
    public enum DepartmentCode
    {
        [Description("CEA")]
        CEA,
        [Description("CMBA")]
        CMBA,
        [Description("CASE")]
        CASE,
        [Description("CNAHS")]
        CNAHS,
        [Description("CCS")]
        CCS,
        [Description("CCJ")]
        CCJ
    }
    [JsonConverter(typeof(EnumDescriptionConverter<DepartmentName>))]
    public enum DepartmentName
    {
        [Description("College of Engineering and Architecture")]
        CEA,
        [Description("College of Management, Business and Accountancy")]
        CMBA,
        [Description("College of Arts, Sciences, and Education")]
        CASE,
        [Description("College of Nursing and Allied Health Sciences")]
        CNAHS,
        [Description("College of Computer Studies")]
        CCS,
        [Description("College of Criminal Justice")]
        CCJ
    }
}

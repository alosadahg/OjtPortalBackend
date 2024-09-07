using OjtPortal.Entities;
using System.ComponentModel;

namespace OjtPortal.Enums
{
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

using OjtPortal.Enums;

namespace OjtPortal.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; } = 0;
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public Department()
        {
        }

        public Department(int departmentID, string departmentCode, string departmentName)
        {
            DepartmentId = departmentID;
            DepartmentCode = departmentCode;
            DepartmentName = departmentName;
        }
    }
}

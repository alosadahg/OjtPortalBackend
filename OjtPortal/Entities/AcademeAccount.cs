using OjtPortal.Enums;

namespace OjtPortal.Entities
{
    public class AcademeAccount : User
    {

        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        

        public AcademeAccount(User user, Department department, string designation) : base(user)
        {
            Department = department;
            Designation = designation;
        }

        public AcademeAccount(AcademeAccount academeAccount): base(academeAccount.FirstName, academeAccount.LastName, academeAccount.UserType, academeAccount.AccountStatus) 
        {
            Department = academeAccount.Department;
            Designation = academeAccount.Designation;
        }

        public AcademeAccount(User user) : base(user)
        {
        }

        public AcademeAccount(): base()
        {
        }
    }
}

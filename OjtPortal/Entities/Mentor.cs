namespace OjtPortal.Entities
{
    public class Mentor : User
    {
        public Company Company { get; set; } = new();
        public int CompanyId { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public IEnumerable<Student>? Students { get; set; }

        public Mentor()
        {
        }

        public Mentor(User user, Company company, string department, string designation): base(user)
        {
            Company = company;
            Department = department;
            Designation = designation;
        }
    }
}

namespace OjtPortal.Entities
{
    public class DegreeProgram
    {
        public int Id { get; set; }
        public Department Department { get; set; } = new();
        public int DepartmentId { get; set; } = 0;
        public string ProgramName { get; set; } = string.Empty;

        public DegreeProgram()
        {
        }

        public DegreeProgram(Department department, string programName)
        {
            Department = department;
            ProgramName = programName;
        }
    }
}

﻿namespace OjtPortal.Entities
{
    public class DegreeProgram
    {
        public int Id { get; set; }
        public Department Department { get; set; } = new();
        public int DepartmentId { get; set; } = 0;
        public string ProgramAlias { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;

        public DegreeProgram() { }
        
    }
}

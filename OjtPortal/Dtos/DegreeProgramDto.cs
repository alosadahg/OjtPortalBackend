﻿using OjtPortal.Entities;

namespace OjtPortal.Dtos
{
    public class DegreeProgramDto
    {
        public int Id { get; set; }
        public string DepartmentCode { get; set; } = string.Empty;
        public int DepartmentId { get; set; } = 0;
        public string ProgramAlias { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
    }
}

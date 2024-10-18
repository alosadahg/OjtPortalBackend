﻿namespace OjtPortal.Dtos
{
    public class SkillDto
    {
    }

    public class NewSkillDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class SkillFrequency
    {
        public string Name { get; set; } = string.Empty;
        public int Usage { get; set; } = 0;
    }
}

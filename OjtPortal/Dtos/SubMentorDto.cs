﻿namespace OjtPortal.Dtos
{
    public class SubMentorDto
    {
        public MentorDto? Submentor { get; set; }
        public MentorDto? HeadMentor { get; set; }
    }

    public class SubMentorWithTasksDto : SubMentorDto
    {
        public List<TaskDto>? TrainingTask { get; set; }
    }
}

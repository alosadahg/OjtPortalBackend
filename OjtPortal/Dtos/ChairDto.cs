using OjtPortal.Entities;
using System.Text.Json.Serialization;

namespace OjtPortal.Dtos
{
    public class ChairDto
    {
        public ExistingUserDto User { get; set; } = new();
        public Department Department { get; set; } = new();
        public string Designation { get; set; } = string.Empty;
        public int StudentCount { get; set; } = 0;
        public int TeacherCount { get; set; } = 0;

        [JsonIgnore]
        public IEnumerable<TeacherDto>? Teachers { get; set; }
        public ChairDto()
        {
            this.TeacherCount = (Teachers != null) ? Teachers.Count() : 0;
            if (Teachers != null)
            {
                foreach (var teacher in Teachers!)
                {
                    this.StudentCount += teacher.StudentCount;
                }
            }
        }
    }
}

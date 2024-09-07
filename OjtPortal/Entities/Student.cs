namespace OjtPortal.Entities
{
    public class Student : AcademeAccount
    {
        public string StudentId { get; set; } = "";
        public DegreeProgram DegreeProgram { get; set; } = new DegreeProgram();
        public int DegreeProgramId { get; set; } = 0;
        public int MentorId { get; set; }
        public Mentor Mentor { get; set; } = new();
        public int InstructorId { get; set; }
        public Teacher Instructor { get; set; } = new();
        public string Division { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly EndDate { get; set; } = new DateOnly();
        public int HrsToRender { get; set; } = 0;
        public int ManDays { get; set; } = 0;
        public InternshipStatus InternshipStatus { get; set; }

        public Student()
        {
        }

        public Student(AcademeAccount academeAccount, string studentId, DegreeProgram degreeProgram, Mentor mentor, Teacher instructor, string division, DateOnly startDate, DateOnly endDate, int hrsToRender, int manDays, InternshipStatus internshipStatus)
            : base(academeAccount)
        {
            StudentId = studentId;
            this.DegreeProgram = degreeProgram;
            Mentor = mentor;
            Instructor = instructor;
            Division = division;
            StartDate = startDate;
            EndDate = endDate;
            HrsToRender = hrsToRender;
            ManDays = manDays;
            InternshipStatus = internshipStatus;
        }
    }
}

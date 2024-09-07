namespace OjtPortal.Entities
{
    public class ShiftRecord
    {
        public int ShiftRecordId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = new();
        public TimeOnly ShiftStart { get; set; } = new();
        public TimeOnly ShiftEnd { get; set; } = new();
        public int DailyDutyHrs { get; set; } = 0;
        public int LateTimeInCount { get; set; } = 0;
        public int AbsencesCount { get; set; } = 0;

        public ShiftRecord()
        {
        }

        public ShiftRecord(Student student, TimeOnly shiftStart, TimeOnly shiftEnd, int dailyDutyHrs, int lateTimeInCount, int absencesCount)
        {
            Student = student;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            DailyDutyHrs = dailyDutyHrs;
            LateTimeInCount = lateTimeInCount;
            AbsencesCount = absencesCount;
        }
    }
}

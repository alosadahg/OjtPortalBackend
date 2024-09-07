namespace OjtPortal.Entities
{
    public class Teacher : AcademeAccount
    {
        public IEnumerable<Student>? Students { get; set; }
        public Teacher() : base()
        {
        }
    }
}

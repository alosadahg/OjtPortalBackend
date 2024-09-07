namespace OjtPortal.Entities
{
    public class Chair : AcademeAccount
    {
        public int ChairId { get; set; }
        public Chair() : base() { }

        public Chair(AcademeAccount academeAccount, int chairId) : base(academeAccount)
        {
            ChairId = chairId;
        }
    }
}

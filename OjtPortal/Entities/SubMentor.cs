using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtPortal.Entities
{
    public class SubMentor : Mentor
    {
        public int HeadMentorId { get; set; }
        public  Mentor? HeadMentor { get; set; }
    }
}

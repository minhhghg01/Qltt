using System.Collections.Generic;

namespace Qltt.Models
{
    public class Teacher : User
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Class Class { get; set; }
    }
}

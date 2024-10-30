using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qltt.Models
{
    public class Teacher : User
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }

        // Navigation property to Class
        [ForeignKey("TeacherId")]
        public virtual Class Class { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}

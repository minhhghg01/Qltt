using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qltt.Models
{
    public class Teacher : User
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }
    }
}

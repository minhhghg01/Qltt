using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qltt.Models
{
    public class Class
    {
        public Class()
        {
            Students = new HashSet<Student>();
            Attendances = new HashSet<Attendance>();
        }

        public int ClassId { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        public string ClassName { get; set; }

        public int? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}

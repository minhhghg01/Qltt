using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qltt.Models
{
    public class Class
    {
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc.")]
        public required string ClassName { get; set; }

        public int TeacherId { get; set; }

        // Navigation properties
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Test> Tests { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }
}

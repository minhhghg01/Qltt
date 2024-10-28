using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qltt.Models
{
    public class Class
    {
        // public int Id { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc.")]
        public required string ClassName { get; set; }

        public int ClassId { get; set; }
        // public Course Course { get; set; }

        public int TeacherId { get; set; }
        // public Teacher Teacher { get; set; }

        // public List<Student> Students { get; set; }
    }
}

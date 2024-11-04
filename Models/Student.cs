using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qltt.Models
{
    public class Student
    {
        public int StudentId { get; set; } // Đây là khóa chính cho bảng Students

        public int UserId { get; set; } // Khóa ngoại đến bảng Users
        [ForeignKey("UserId")]
        public virtual User User { get; set; } // Navigation property

        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public virtual ICollection<StudentTest> StudentTests { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }

        [NotMapped]
        public int AttendanceCount { get; set; } // Tổng số buổi

        [NotMapped]
        public int PresentCount { get; set; }    // Số buổi có mặt
    }
}

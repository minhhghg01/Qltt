using System.ComponentModel.DataAnnotations.Schema;

namespace Qltt.Models
{
    public class Student : User
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }
        public virtual ICollection<StudentTest> StudentTests { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}

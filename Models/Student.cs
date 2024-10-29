namespace Qltt.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }

        public virtual User User { get; set; }
        public virtual Class Class { get; set; }
        public virtual ICollection<StudentTest> StudentTests { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}

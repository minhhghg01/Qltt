namespace Qltt.Models
{
    public class Test
    {
        public int TestId { get; set; }
        public int ClassId { get; set; }
        public string TestName { get; set; }

        // Navigation properties
        public Class Class { get; set; }
        public virtual ICollection<StudentTest> StudentTests { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}

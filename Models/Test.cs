namespace Qltt.Models
{
    public class Test
    {
        public int TestId { get; set; }
        public int ClassId { get; set; }
        public string TestName { get; set; }
        public DateTime Date { get; set; }

        // Navigation properties
        public Class Class { get; set; }
        public ICollection<StudentTest> StudentTests { get; set; }
    }
}

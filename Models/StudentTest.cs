namespace Qltt.Models
{
    public class StudentTest
    {
        public int StudentTestId { get; set; }
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public decimal Score { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Test Test { get; set; }
    }
}

namespace Qltt.Models
{
    public class Admin : User
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
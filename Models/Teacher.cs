using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qltt.Models
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Đặt TeacherId là tự tăng
        public int TeacherId { get; set; }

        [Required]
        public int UserId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")] 
        public virtual User User { get; set; }
        // Thay đổi thành một quan hệ một-nhiều
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}

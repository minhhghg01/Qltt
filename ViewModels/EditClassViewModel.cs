using Qltt.Models;

namespace Qltt.ViewModels
{
    public class EditClassViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int? TeacherId { get; set; } // Giáo viên có thể không được chỉ định
        public List<Teacher> Teachers { get; set; } // Danh sách giáo viên
    }
}


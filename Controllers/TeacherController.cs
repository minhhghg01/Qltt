using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Qltt.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        public IActionResult Index() { return View(); }
        // Quản lý học sinh
        public IActionResult ManageStudents() { return View(); }

        // Quản lý lớp học
        public IActionResult ManageClasses() { return View(); }

        // Quản lý môn học
        public IActionResult ManageSubjects() { return View(); }

        // Thêm các action methods khác nếu cần
    }
}

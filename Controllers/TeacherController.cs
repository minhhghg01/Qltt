using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Qltt.Data;
using Microsoft.EntityFrameworkCore;

namespace Qltt.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var user = await _context.Users.FindAsync(userId);
            return View(user);
        }

        // Quản lý học sinh trong lớp
        public async Task<IActionResult> ManageClass() {
            var classId = User.FindFirst("ClassId")?.Value;
            var students = await _context.Students.Where(s => s.ClassId == int.Parse(classId)).ToListAsync();
            return View(students);
        }

        // Quản lý môn học
        public IActionResult ManageSubjects() { return View(); }

        // Thêm các action methods khác nếu cần
    }
}

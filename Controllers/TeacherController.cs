using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Qltt.Data;
using Microsoft.EntityFrameworkCore;
using Qltt.Models;

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

        // Điểm danh học sinh
        public async Task<IActionResult> Attendance()
        {
            // Đoạn mã lấy classId như trước
            var classId = User.FindFirst("ClassId")?.Value;
            if (string.IsNullOrEmpty(classId))
            {
                classId = "1"; // Mặc định là 1
            }

            if (!int.TryParse(classId, out int classIdInt))
            {
                return BadRequest("Invalid ClassId.");
            }

            // Lấy danh sách học sinh trong lớp dựa trên ClassId
            var students = await _context.Students
            .Include(s => s.User)
            .Where(s => s.ClassId == classIdInt)
            .ToListAsync();

            // Lấy tên lớp học
            var className = await _context.Classes
                .Where(c => c.ClassId == classIdInt)
                .Select(c => c.ClassName) // Giả sử bạn có trường ClassName trong bảng Classes
                .FirstOrDefaultAsync();

            ViewBag.ClassName = className;

            // Tính tổng số buổi và số buổi có mặt
            foreach (var student in students)
            {
                var attendanceCount = await _context.Attendances
                    .CountAsync(a => a.StudentId == student.StudentId);
                var presentCount = await _context.Attendances
                    .CountAsync(a => a.StudentId == student.StudentId && a.IsPresent);
            }

            return View(students);
        }

        // Get MarkAttendance
        public async Task<IActionResult> MarkAttendanceAsync()
        {
             var classId = User.FindFirst("ClassId")?.Value;
            if (string.IsNullOrEmpty(classId))
            {
                classId = "1"; // Mặc định là 1
            }

            if (!int.TryParse(classId, out int classIdInt))
            {
                return BadRequest("Invalid ClassId.");
            }

            // Lấy danh sách học sinh trong lớp dựa trên ClassId
            var students = await _context.Students
            .Include(s => s.User)
            .Where(s => s.ClassId == classIdInt)
            .ToListAsync();

            // Lấy tên lớp học
            var className = await _context.Classes
                .Where(c => c.ClassId == classIdInt)
                .Select(c => c.ClassName) // Giả sử bạn có trường ClassName trong bảng Classes
                .FirstOrDefaultAsync();

            ViewBag.ClassName = className;

            // Tính tổng số buổi và số buổi có mặt
            foreach (var student in students)
            {
                var attendanceCount = await _context.Attendances
                    .CountAsync(a => a.StudentId == student.StudentId);
                var presentCount = await _context.Attendances
                    .CountAsync(a => a.StudentId == student.StudentId && a.IsPresent);
            }

            return View(students);
        }

        // Post MarkAttendance
        [HttpPost]
        [ActionName("MarkAttendance")]
        public async Task<IActionResult> MarkAttendance(List<int> studentIds, List<bool> isPresent)
        {
            // Kiểm tra dữ liệu điểm danh
            if (studentIds == null || isPresent == null || studentIds.Count != isPresent.Count)
            {
                return BadRequest("Invalid attendance data.");
            }

            // Thêm bản ghi điểm danh vào cơ sở dữ liệu
            for (int i = 0; i < studentIds.Count; i++)
            {
                var attendance = new Attendance
                {
                    StudentId = studentIds[i],
                    ClassId = 1,
                    Date = DateTime.Now,
                    IsPresent = isPresent[i],
                    Remarks = isPresent[i] ? "Có mặt" : "Vắng mặt"
                };

                _context.Attendances.Add(attendance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Attendance"); // Quay lại trang chỉ định sau khi điểm danh
        }

    }
}

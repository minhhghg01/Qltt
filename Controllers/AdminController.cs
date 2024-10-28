using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Qltt.Data;
using Microsoft.EntityFrameworkCore;

namespace Qltt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes.ToListAsync();
            return View(classes);
        }

        //------------------ Quản lý lớp học ------------------
        public async Task<IActionResult> ManageClasses()
        {
            var classes = await _context.Classes.ToListAsync(); // Giả sử bạn lấy danh sách lớp
            if (classes == null)
            {
                return NotFound(); // Xử lý nếu không tìm thấy lớp nào
            }
            return View(classes); // Truyền danh sách lớp vào view
        }

        public IActionResult CreateClass() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClass([Bind("ClassId,ClassName")] Models.Class cls)
        {
            if (ModelState.IsValid)
            {
                _context.Classes.Add(cls);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageClasses));
            }
            return View(cls);
        }

        public async Task<IActionResult> DetailsClass(int id)
        {
            var classDetail = await _context.Classes.FirstOrDefaultAsync(c => c.ClassId == id);
            if (classDetail == null)
            {
                return NotFound();
            }
            return View(classDetail);
        }

        public async Task<IActionResult> DeleteClass(int id)
        {
            var classDelete = await _context.Classes.FirstOrDefaultAsync(c => c.ClassId == id);
            if (classDelete == null)
            {
                return NotFound();
            }
            // _context.Classes.Remove(classDelete);
            // await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageClasses));
        }

        [HttpPost, ActionName("DeleteClass")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClassConfirmed(int id)
        {
            var classDelete = await _context.Classes.FindAsync(id);
            if (classDelete != null)
            {
                _context.Classes.Remove(classDelete); // Xóa lớp khỏi DB
            }
            await _context.SaveChangesAsync(); // Lưu thay đổi

            return RedirectToAction(nameof(ManageClasses));
        }

        //------------------ Quản lý giáo viên ------------------
        public async Task<IActionResult> ManageTeachers() {
            var teachers = await _context.Teachers.ToListAsync(); // Đảm bảo dữ liệu không null
            return View(teachers);
         }

        public IActionResult AddTeacher() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeacher([Bind("FirstName,LastName,Email,ClassId,Password")] Models.Teacher teacher) { 
            if (ModelState.IsValid)
            {
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageTeachers));
            }
            return View(teacher);
        }

        public async Task<IActionResult> DeleteTeacher(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync   (t => t.Id == id);
        if (teacher == null)
        {
            // return NotFound(); // Nếu không tìm thấy giáo viên
        }
            return View(teacher); // Truyền giáo viên vào View
        }

        [HttpPost, ActionName("DeleteTeacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);
        if (teacher != null)
        {
                _context.Teachers.Remove(teacher); // Xóa giáo viên khỏi DB
            }
            await _context.SaveChangesAsync(); // Lưu thay đổi
        
            return RedirectToAction("ManageTeachers"); // Quay lại danh sách giáo viên
        }

        //------------------ Quản lý học sinh ------------------
        public IActionResult ManageStudents() { return View(); }

        //------------------ Quản lý môn học ------------------
        public IActionResult ManageSubjects() { return View(); }

        // Thêm các action methods khác nếu cần
    }
}

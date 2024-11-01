using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Qltt.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Qltt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes.ToListAsync();
            return View(classes);
        }

        //------------------ Quản lý lớp học ------------------
        public async Task<IActionResult> ManageClasses(int page = 1, int pageSize = 10)
        {
            var classes = await _context.Classes.ToListAsync(); // Giả sử bạn lấy danh sách lớp
            if (classes == null)
            {
                return NotFound(); // Xử lý nếu không tìm thấy lớp nào
            }
            var pagedClasses = classes.ToPagedList(page, pageSize);
            return View(pagedClasses); // Truyền danh sách lớp vào view
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
            var classDetail = await _context.Classes
            .Include(c => c.Students)
            .Include(c => c.Teacher)
            .ThenInclude(t => t.User)
            .FirstOrDefaultAsync(c => c.ClassId == id);
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
        // Get Manage Teachers
        public async Task<IActionResult> ManageTeachers(int page = 1, int pageSize = 10)
        {
            var teachers = await _context.Teachers
            .Include(t => t.Classes)
            .Include(t => t.User)
            .ToListAsync(); // Đảm bảo dữ liệu không null
            var pagedTeachers = teachers.ToPagedList(page, pageSize);
            return View(pagedTeachers);
        }

        // Get Add Teacher
        public IActionResult AddTeacher() { return View(); }

        // Post Add Teacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeacher(
            string email,
            string firstName,
            string lastName,
            string password,
            string className
        )
        {
                    // Tạo User mới
            var user = new Models.User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                Role = "Teacher",
            };

            // Thêm User vào cơ sở dữ liệu
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Tạo Teacher mới với UserId của User
            var newTeacher = new Models.Teacher
            {
                UserId = user.UserId,
            };

            _context.Teachers.Add(newTeacher);
            await _context.SaveChangesAsync();

            // Tạo một Class mới mà không cần TeacherId
            var newClass = new Models.Class
            {
                ClassName = className,
                TeacherId = newTeacher.TeacherId,
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageTeachers));
        }

        // Get Delete Teacher
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // Post Delete Teacher
        [HttpPost, ActionName("DeleteTeacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacherConfirmed(int id)
        {
            var teacher = await _context.Teachers
        .Include(t => t.User)
                .Include(t => t.Classes) // Thêm Include Classes
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Xóa các Class liên kết với Teacher
            foreach (var cls in teacher.Classes)
            {
                cls.TeacherId = null;
            }

            _context.Teachers.Remove(teacher);

            // Xóa User
            if (teacher.User != null)
    {
                _context.Users.Remove(teacher.User);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageTeachers));
        }

        //------------------ Quản lý học sinh ------------------
        public async Task<IActionResult> ManageStudents(int page = 1, int pageSize = 10)
        {
            var students = await _context.Students
        .Include(s => s.Class)
                .Include(s => s.User)
                .ToListAsync();
            var pagedStudents = students.ToPagedList(page, pageSize);
            return View(pagedStudents);
        }

    }
}

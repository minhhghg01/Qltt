using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Qltt.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.Data.SqlClient;

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

        // Get Create Class
        public IActionResult CreateClass() { return View(); }

        // Post Create Class
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClass(string className)
        {
            var newClass = new Models.Class
            {
                ClassName = className,
            };
            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageClasses));
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

        // Get Delete Class
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classDelete = await _context.Classes
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.ClassId == id);
            if (classDelete == null)
            {
                return NotFound();
            }
            return View(classDelete);
        }

        // Post Delete Class
        [HttpPost, ActionName("DeleteClass")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClassConfirmed(int id)
        {
            var classDelete = await _context.Classes
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.ClassId == id);
            if (classDelete != null)
            {
                _context.Classes.Remove(classDelete); // Xóa lớp khỏi DB
            }

            // Xóa các Student trong lớp
            if (classDelete.Students != null)
            {
                _context.Students.RemoveRange(classDelete.Students);
            }

            await _context.SaveChangesAsync(); // Lưu thay đổi

            return RedirectToAction(nameof(ManageClasses));
        }

        // Get Edit Class
        public async Task<IActionResult> EditClass(int id)
        {
            var classEdit = await _context.Classes.FindAsync(id);
            return View(classEdit);
        }

        // Post Edit Class
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClass(int id, string className)
        {
            var classEdit = await _context.Classes
            .Include(c => c.Teacher)
            .FirstOrDefaultAsync(c => c.ClassId == id);

            if (classEdit == null)
            {
                return NotFound();
            }

            classEdit.ClassName = className;
            // classEdit.TeacherId = classEdit.Teacher.TeacherId;

            await _context.SaveChangesAsync();
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

            // Tạo một Class mới 
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

        // Get Edit Teacher
        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            return View(teacher);
        }

        // Post Edit Teacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacher(int id, string firstName, string lastName, string email, string password)
        {
            // Tìm Teacher và User liên kết từ cơ sở dữ liệu
            var existingTeacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (existingTeacher == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Sử dụng câu lệnh SQL thuần để cập nhật User
                    var sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Password = @Password WHERE UserId = @UserId";
                    await _context.Database.ExecuteSqlRawAsync(sql,
                        new SqlParameter("@FirstName", firstName ?? (object)DBNull.Value),
                        new SqlParameter("@LastName", lastName ?? (object)DBNull.Value),
                        new SqlParameter("@Email", email ?? (object)DBNull.Value),
                        new SqlParameter("@Password", password ?? (object)DBNull.Value),
                        new SqlParameter("@UserId", existingTeacher.UserId));

                    return RedirectToAction(nameof(ManageTeachers));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Teachers.Any(t => t.TeacherId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(existingTeacher);
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


        // Get Add Student
        public IActionResult AddStudent() { return View(); }

        // Post Add Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(
            string email,
            string firstName,
            string lastName,
            string password,
            string className
        )
        {
            // Kiểm tra nếu `email` bị null hoặc rỗng
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("Email", "Email là bắt buộc.");
                return View(); // Trả về view hiện tại và hiển thị lỗi
            }

            // Thêm User vào cơ sở dữ liệu
            var user = new Models.User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = password,
                Role = "Student",
            };

            // Tạo users
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Tìm Class từ cơ sở dữ liệu
            var classStudent = await _context.Classes.FirstOrDefaultAsync(c => c.ClassName == className);


            // Kiểm tra nếu không tìm thấy Class
            if (classStudent == null)
            {
                ModelState.AddModelError("ClassName", "Không tìm thấy lớp học.");
                return View(); // Trả về view hiện tại và hiển thị lỗi
            }

            // Thêm Student vào cơ sở dữ liệu
            var insertStudentQuery = "INSERT INTO Students (UserId, ClassId) VALUES (@UserId, @ClassId)";
            await _context.Database.ExecuteSqlRawAsync(insertStudentQuery,
                new SqlParameter("@UserId", user.UserId),
                new SqlParameter("@ClassId", classStudent.ClassId)
            );

            return RedirectToAction(nameof(ManageStudents));
        }


        // Get Delete Student
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Post Delete Student
        [HttpPost, ActionName("DeleteStudent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudentConfirmed(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            // Xóa Student
            _context.Students.Remove(student);

            // Xóa User
            if (student.User != null)
            {
                _context.Users.Remove(student.User);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageStudents));
        }

        // Get Edit Student
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            return View(student);
        }

        // Post Edit Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(int id, string firstName, string lastName, string email, string password)
        {
            // Tìm Student và User liên kết từ cơ sở dữ liệu
            var existingStudent = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == id);
            
            Console.WriteLine("hello" + existingStudent.UserId + " " + existingStudent.StudentId + " " + id);

            if (existingStudent == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Sử dụng câu lệnh SQL thuần để cập nhật User
                    var sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Password = @Password WHERE UserId = @UserId";
                    await _context.Database.ExecuteSqlRawAsync(sql,
                        new SqlParameter("@FirstName", firstName ?? (object)DBNull.Value),
                        new SqlParameter("@LastName", lastName ?? (object)DBNull.Value),
                        new SqlParameter("@Email", email ?? (object)DBNull.Value),
                        new SqlParameter("@Password", password ?? (object)DBNull.Value),
                        new SqlParameter("@UserId", existingStudent.UserId));

                    return RedirectToAction(nameof(ManageStudents));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(s => s.StudentId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(existingStudent);
        }

    }
}

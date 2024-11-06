using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qltt.Data;
using Qltt.Models;

namespace Qltt.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentController> _logger;

        public StudentController(ApplicationDbContext context, ILogger<StudentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var user = await _context.Users.FindAsync(userId);
            return View(user);
        }

        public async Task<IActionResult> Tests()
        {
            try
            {
                // Lấy classId từ student hiện tại
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    return NotFound("Không tìm thấy thông tin học sinh!");
                }

                // Lấy danh sách bài test của lớp đó
                var tests = await _context.Tests
                    .Where(t => t.ClassId == student.ClassId)
                    .OrderBy(t => t.TestId)
                    .ToListAsync();

                return View(tests);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return View(new List<Test>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Start(int testId = 3)
        {
            try
            {
                _logger.LogInformation($"Fetching questions for TestId={testId}, ClassId=1");

                var questions = await _context.Questions
                    .Include(q => q.Test)
                    .Where(q => q.TestId == testId && q.Test.ClassId == 1)
                    .OrderBy(r => Guid.NewGuid())
                    .Take(10)
                    .ToListAsync();

                if (!questions.Any())
                {
                    _logger.LogWarning($"No questions found for TestId={testId}, ClassId=1");
                    TempData["Error"] = "Không tìm thấy câu hỏi nào!";
                    return RedirectToAction("Tests");
                }

                _logger.LogInformation($"Found {questions.Count} questions");
                return View(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Tests");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] SubmitModel model)
        {
            try 
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    return BadRequest("Không tìm thấy thông tin học sinh!");
                }

                decimal score = ((decimal)model.correctCount / model.totalQuestions) * 100;

                var studentTest = new StudentTest
                {
                    StudentId = student.StudentId,
                    TestId = model.testId,
                    Score = score, 
                };

                _context.StudentTests.Add(studentTest);
                await _context.SaveChangesAsync();

                // Trả về URL để redirect
                var resultUrl = Url.Action("Result", "Student", new { id = studentTest.StudentTestId });
                return Json(new { success = true, redirectUrl = resultUrl });
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi
                Console.WriteLine($"Error details: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<IActionResult> Result(int id)
        {
            try
            {
                // Lấy kết quả bài thi từ database
                var studentTest = await _context.StudentTests
                    .Include(st => st.Test)  // Include thêm thông tin của Test nếu cần
                    .FirstOrDefaultAsync(st => st.StudentTestId == id);

                if (studentTest == null)
                {
                    return NotFound("Không tìm thấy kết quả bài kiểm tra!");
                }

                // Log để debug
                Console.WriteLine($"Found StudentTest: Id={studentTest.StudentTestId}, Score={studentTest.Score}");

                return View(studentTest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Result action: {ex.Message}");
                return RedirectToAction("Tests");
            }
        }
    }

    public class SubmitModel
    {
        public int correctCount { get; set; }
        public int totalQuestions { get; set; }
        public int testId { get; set; }
    }
}


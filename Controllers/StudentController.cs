

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

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var user = await _context.Users.FindAsync(userId);
            return View(user);
        }

        public async Task<IActionResult> Tests()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tests = await _context.Tests
                .Include(t => t.StudentTests)
                .Where(t => t.Date.Date >= DateTime.Today)
                .ToListAsync();

            return View(tests);
        }

        public async Task<IActionResult> Start(int testId)
        {
            // var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // // Lấy 10 câu hỏi ngẫu nhiên
            // var questions = await _context.Questions
            //     .OrderBy(r => Guid.NewGuid())
            //     .Take(10)
            //     .ToListAsync();

            // // Lưu thông tin vào Session để kiểm tra khi nộp bài
            // HttpContext.Session.SetString("TestQuestions", JsonSerializer.Serialize(questions));
            // HttpContext.Session.SetInt32("TestId", testId);
            // HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());

            // return View(questions);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(Dictionary<int, string> answers)
        {
            var testId = HttpContext.Session.GetInt32("TestId");
            var questionsJson = HttpContext.Session.GetString("TestQuestions");
            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (testId == null || questionsJson == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var questions = JsonSerializer.Deserialize<List<Question>>(questionsJson);

            // Tính điểm
            decimal score = 0;
            foreach (var question in questions)
            {
                if (answers.ContainsKey(question.QuestionId) &&
                    answers[question.QuestionId] == question.CorrectAnswer)
                {
                    score += 1;
                }
            }

            // Quy đổi sang thang điểm 10
            score = (score / 10) * 10;

            // Lưu kết quả
            var studentTest = new StudentTest
            {
                StudentId = studentId,
                TestId = testId.Value,
                Score = score
            };

            _context.StudentTests.Add(studentTest);
            await _context.SaveChangesAsync();

            // Xóa dữ liệu session
            HttpContext.Session.Remove("TestQuestions");
            HttpContext.Session.Remove("TestId");

            return RedirectToAction("Result", new { id = studentTest.StudentTestId });
        }

        public async Task<IActionResult> Result(int id)
        {
            // var studentTest = await _context.StudentTests
            //     .Include(st => st.Test)
            //     .FirstOrDefaultAsync(st => st.StudentTestId == id);

            // if (studentTest == null)
            // {
            //     return NotFound();
            // }

            return View();
        }
    }
}


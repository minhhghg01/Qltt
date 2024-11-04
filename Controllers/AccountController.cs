using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Qltt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Qltt.Models;

namespace Qltt.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login()
        {
            // Kiểm tra nếu user đã đăng nhập thì chuyển hướng
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            try 
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    TempData["Error"] = "Vui lòng nhập email và mật khẩu";
                    return View();
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null && user.Password == password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim(ClaimTypes.GivenName, user.FirstName),
                        new Claim(ClaimTypes.Surname, user.LastName),
                    };

                    // Xử lý student với try-catch riêng
                    try 
                    {
                        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.UserId);
                        if (student != null)
                        {
                            Console.WriteLine($"Found student - StudentId: {student.StudentId}, ClassId: {student.ClassId}");
                            if (student.ClassId > 0)
                            {
                                claims.Add(new Claim("ClassId", student.ClassId.ToString()));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing student data: {ex.Message}");
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                        // Thêm các options cho cookie
                        AllowRefresh = true
                    };

                    try 
                    {
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);
                        
                        HttpContext.Session.SetString("UserRole", user.Role);
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during sign in: {ex.Message}");
                        TempData["Error"] = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.";
                        return View();
                    }
                }

                TempData["Error"] = "Email hoặc mật khẩu không chính xác";
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in Login: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                return View();
            }
        }


        public async Task<IActionResult> Logout()
        {
            // Đăng xuất
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Xóa cache
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Chuyển hướng về trang login
            return RedirectToAction("Login", "Account");
        }
    }
}

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
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập email và mật khẩu");
                    return View();
                }

                // Tìm user trong bảng Users
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    switch (user.Role.ToLower())
                    {
                        case "admin":
                            return await SignInUser(user.Email, "Admin", user.FirstName, "/Admin/Index");
                        case "teacher":
                            return await SignInUser(user.Email, "Teacher", user.FirstName, "/Teacher/Index");
                        default:
                            ModelState.AddModelError(string.Empty, "Vai trò người dùng không hợp lệ");
                            return View();
                    }
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác");
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình đăng nhập");
                return View();
            }
        }

        private async Task<IActionResult> SignInUser(string email, string role, string firstName, string redirectUrl)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, firstName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            HttpContext.Session.SetString("UserRole", role);

            return Redirect(redirectUrl);
        }

        public async Task<IActionResult> Logout()
        {
            // Xóa session
            HttpContext.Session.Clear();
            
            // Xóa authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Xóa tất cả cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            // Thêm headers chống cache
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, private";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            return RedirectToAction("Login", "Account");
        }
    }
}

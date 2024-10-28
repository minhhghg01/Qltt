using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Qltt.Data;
using Microsoft.AspNetCore.Authorization;

// [Authorize]
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
    public async Task<IActionResult> Login(string email, string password)
    {
        // Kiểm tra thông tin đăng nhập cho từng loại tài khoản
        var admin = _context.Admins.FirstOrDefault(a => a.Email == email && a.Password == password);
        var teacher = _context.Teachers.FirstOrDefault(t => t.Email == email && t.Password == password);
        // var student = _context.Students.FirstOrDefault(s => s.Email == email && s.PasswordHash == password);

        if (admin != null)
            return await SignInUser(admin.Email, "Admin", "/Admin/Index");
        else if (teacher != null)
            return await SignInUser(teacher.Email, "Teacher", "/Teacher/Index");
        // else if (student != null)
        //     return await SignInUser(student.Email, "Student", "/Student/Index");

        ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không chính xác.");
        return View();
    }

    private async Task<IActionResult> SignInUser(string email, string role, string redirectUrl)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
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

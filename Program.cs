using Microsoft.EntityFrameworkCore;
using Qltt.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session hết hạn sau 30 phút
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Kiểm tra kết nối cơ sở dữ liệu
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Thực hiện một truy vấn đơn giản
        dbContext.Database.CanConnect(); // Kiểm tra kết nối
        Console.WriteLine("Kết nối đến cơ sở dữ liệu thành công.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi kết nối đến cơ sở dữ liệu: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

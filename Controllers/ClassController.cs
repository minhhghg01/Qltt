using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qltt.Data;

namespace Qltt.Controllers
{
    public class ClassController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes.ToListAsync();
            return View(classes);
        }

        public async Task<IActionResult> Details(int id)
    {
        var classInfo = await _context.Classes
            // .Include(c => c.Students)
            // .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (classInfo == null)
        {
            return NotFound();
        }

            return View(classInfo);
        }


    }
}
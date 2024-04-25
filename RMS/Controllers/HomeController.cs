using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models;
using RMS.ViewModels;
using System.Diagnostics;

namespace RMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {

            if (id != null)
            {
                return View(new HomeViewModel
                {
                    Categories = await _context.Categories.ToListAsync(),
                    MenuItems = await _context.MenuItems.Where(c => c.CategoryId == id).ToListAsync(),
                    Category = await _context.Categories.FindAsync(id)
                });
            }

            return View(new HomeViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                MenuItems = await _context.MenuItems.ToListAsync()
            });
        }

        public async Task<IActionResult> Item(int id)
        {
            return View((await _context.MenuItems.Include(m=>m.Category).ToListAsync()).Find(i => i.Id.Equals(id)));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
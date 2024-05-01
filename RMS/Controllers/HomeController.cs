using Microsoft.AspNetCore.Authorization;
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
            return View((await _context.MenuItems.Include(m=>m.Category).Include(m=>m.Reviews).ThenInclude(r=>r.Customer)
                .Include(m=>m.Stocks)
                .ToListAsync()).Find(i => i.Id.Equals(id)));
        }

        [Authorize]
        public async Task<IActionResult> Orders()
        {
            return View(await _context.Orders
                .Include(o => o.Reservation)
                .Include(M=>M.Promotion).Include(m => m.OrderItems).ThenInclude(oi=>oi.MenuItem).Where(o=>o.CustomerId == User.GetUserId()).ToListAsync());
        }


        [HttpGet]
        [Authorize]
        public IActionResult Review(int id)
        {
            return View(_context.OrderItems.Include(oi=>oi.MenuItem).FirstOrDefault(i => i.Id == id));
        }

        [HttpGet]
        [Authorize]
        public IActionResult SubmitReview(int id, string txt)
        {
            var item = _context.OrderItems.FirstOrDefault(i => i.Id == id);

            if (item is null)
            {
                return RedirectToAction("Orders", "Home");
            }

            _context.Reviews.Add(new Review
            {
                ItemId = item.MenuItemId,
                CustomerId = User.GetUserId()!,
                Date = DateTime.Now,
                Text = txt
            });

            item.Reviewed = true;

            _context.SaveChanges();

            return RedirectToAction("Orders", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
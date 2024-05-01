using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models;

namespace RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.Customer).Include(o => o.Payment).Include(o => o.Promotion).Include(o=>o.OrderItems).ThenInclude(oi=>oi.MenuItem);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Promotion)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult MarkComplete(int id)
        {
            var order = _context.Orders.FirstOrDefault(m => m.Id == id);

            if(order is not null)
            {
                order.Status = OrderStatus.Completed;

                var payment = _context.Payments.FirstOrDefault(p => p.OrderId == id);
                if (payment is not null)
                {
                    payment.Status = PaymentStatus.Paid;
                }

                _context.SaveChanges();
            }
           

            return RedirectToAction("Index");
        }
        public IActionResult MarkCancel(int id)
        {
            var order = _context.Orders.FirstOrDefault(m => m.Id == id);

            if (order is not null)
            {
                order.Status = OrderStatus.Cancelled;

                var payment = _context.Payments.FirstOrDefault(p => p.OrderId == id);
                if(payment is not null)
                {
                    payment.Status = PaymentStatus.Refunded;
                }

                _context.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RMS.Data;
using RMS.Models;
using System.Security.Claims;

namespace RMS.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CartController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult Add([FromForm] CartItem item)
        {
            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            SaveCart(cart);
            return Ok();
        }

        public IActionResult Delete(int id)
        {
            var cart = GetCart();

            cart.RemoveAll(i => i.Id == id);

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Checkout(int id, string orderType, string promoCode, int guestCount)
        {
            Table? table = null;
            if(Convert.ToInt32(orderType) == (int)OrderType.DineIn)
            {
                if(guestCount == 0)
                {
                    TempData["Error"] = "Please enter the number of guests for Dine in orders.";
                    return RedirectToAction("Index");
                }

                table = _context.Tables.Include(t=>t.Reservations).FirstOrDefault(t=>t.Capacity >= guestCount && t.Reservations.All(r=>r.Date < DateTime.Now));

                if (table is null)
                {
                    TempData["Error"] = "No tables available at the moment. Come back later!";
                    return RedirectToAction("Index");
                }
            }

            var cart = GetCart();

            bool isCashPayment = id == 0;

            var orderItems = new List<OrderItem>();

            if(cart.Any())
            {
                decimal total = 0;
                foreach (var item in cart)
                {
                    total += item.Price * item.Quantity;
                    orderItems.Add(new OrderItem
                    {
                        MenuItemId = item.Id,
                        Quantity = item.Quantity,
                    });

                    // Handled by DecrementStockOnOrder Tirgger now 
                    //var stock = _context.Stocks.OrderByDescending(s=>s.Quantity).FirstOrDefault(s => s.MenuItemId == item.Id);
                    //if(stock is not null) stock.Quantity -= item.Quantity;
                }

                Promotion? promotion = null;
                if (!string.IsNullOrEmpty(promoCode))
                {
                    promotion = _context.Promotions.FirstOrDefault(p => p.Code == promoCode);
                    if(promotion != null)
                    {
                        total -= (promotion.DiscountPercentage/100) * total;
                    }
                }

                var order = new Order
                {
                    CustomerId = User.GetUserId()!,
                    Date = DateTime.Now,
                    OrderItems = orderItems,
                    Status = OrderStatus.Pending,
                    Type = (OrderType)Convert.ToInt32(orderType),
                    PromotionId = promotion?.Id,
                };

                _context.Orders.Add(order);

                _context.SaveChanges();

                if(table is not null)
                {
                    var reservation = new Reservation
                    {
                        Date = DateTime.Now.AddMinutes(30),
                        TableId = table.Id,
                        OrderId = order.Id,
                        Status = ReservationStatus.Reserved,
                    };
                    table.Reservations.Add(reservation);

                    _context.SaveChanges();

                    order.ReservationId = reservation.Id;
                }

                _context.Payments.Add(new Payment
                {
                    Amount = total,
                    Date = DateTime.Now,
                    Status = isCashPayment ? PaymentStatus.Pending : PaymentStatus.Paid,
                    OrderId = order.Id,
                    Method = isCashPayment ? PaymentMethod.Cash : PaymentMethod.Card
                });

                _context.SaveChanges();
            }

            ClearCart();

            return RedirectToAction("Orders", "Home");
        }

        private List<CartItem> GetCart()
        {
            // Retrieve cart from cookies
            var cartJson = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            if (cartJson != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }
            else
            {
                return new List<CartItem>();
            }
        }
        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            _httpContextAccessor.HttpContext.Response.Cookies.Append("cart", cartJson, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1) // Cart expires in 1 day
            });
        }

        private void ClearCart()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("cart");
        }
    }
}

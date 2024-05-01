using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Checkout(int id, string orderType, string promoCode)
        {
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

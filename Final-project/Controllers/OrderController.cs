using Final_project.Data;
using Final_project.Entities;
using Final_project.Models;
using Final_project.Response;
using Final_project.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Controllers
{
    public class OrderController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IBasketService _basketService;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _accessor;

        public OrderController(MichaelDbContext context, IOrderService orderService, IBasketService basketService, UserManager<User> userManager, IHttpContextAccessor accessor)
        {
            _context = context;
            _orderService = orderService;
            _basketService = basketService;
            _userManager = userManager;
            _accessor = accessor;
        }

        public async Task<IActionResult> OrderHistory()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            User user = await _userManager.GetUserAsync(_accessor.HttpContext.User);

            if (user is null) return RedirectToAction("Login", "Account");

            ViewBag.Orders = await _orderService.GetOrdersByUserName(user.UserName);
            return View(user);
        }
        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            ViewBag.User = await _userManager.GetUserAsync(_accessor.HttpContext.User);
            ViewBag.Basket = await _basketService.GetBasket();
            if (ViewBag.Basket.BasketItems.Count <= 0)
            {
                return RedirectToAction("Cart", "Basket");
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(OrderViewModel model)
        {
            TempData["Order"] = false;
            CommonResponse response = await _orderService.CreateAsync(model);
            ViewBag.User = await _userManager.GetUserAsync(_accessor.HttpContext.User);
            ViewBag.Basket = await _basketService.GetBasket();

            if (response.StatusCode == 200)
            {
                TempData["Order"] = true;
                return RedirectToAction("Index", "Home", new { success = true, orderId = response.Data });
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    foreach (string message in ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage))
                    {
                        ModelState.AddModelError("", message);
                    }
                    return View(model);
                }
                TempData["Order"] = true;
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}

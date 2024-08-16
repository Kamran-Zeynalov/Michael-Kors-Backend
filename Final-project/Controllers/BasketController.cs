using Final_project.Data;
using Final_project.Entities;
using Final_project.Services.Implementations;
using Final_project.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace Final_project.Controllers
{
    public class BasketController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IWishlistService _wishlistService;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<User> _userManager;
        private readonly IBasketService _basketService;

        public BasketController(MichaelDbContext context, IWishlistService wishlistService, IHttpContextAccessor http, UserManager<User> userManager, IBasketService basketService)
        {
            _context = context;
            _wishlistService = wishlistService;
            _http = http;
            _userManager = userManager;
            _basketService = basketService;
        }
        public async Task<IActionResult> Cart()
        {
            if (_http.HttpContext.User.Identity.IsAuthenticated)
            {
                return View(await _basketService.GetBasket());
            }
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Favorite()
        {
            if (_http.HttpContext.User.Identity.IsAuthenticated)
            {
                return View(await _wishlistService.GetWishlist());
            }
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> RemoveBasket(int id, int colorId, int sizeId)
        {
            TempData["Basket"] = false;
            await _basketService.RemoveBasketItem(id, colorId, sizeId);
            await _wishlistService.AddWishlist(id);
            TempData["Basket"] = true;
            return RedirectToAction("Favorite", "Basket");
        }
    }
}

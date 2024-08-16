using Final_project.Data;
using Final_project.Entities;
using Final_project.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class CheckoutController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IOrderService _orderService;

        public CheckoutController(MichaelDbContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            List<Order> orders = _orderService.GetAll();
            return View(orders);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id == 0) return NotFound();
            Order order = await _orderService.Get(id);
            if (order is null) return BadRequest();
            return View(order);
        }

        public async Task<IActionResult> Accept(int id)
        {
            await _orderService.Accept(id);
            return RedirectToAction("Index", "Checkout");
        }

        public async Task<IActionResult> Reject(int id)
        {
            await _orderService.Reject(id);
            return RedirectToAction("Index", "Checkout");
        }
    }
}

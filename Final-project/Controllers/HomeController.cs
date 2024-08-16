using Final_project.Data;
using Final_project.Entities;
using Final_project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace Final_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly MichaelDbContext _context;

        public HomeController(MichaelDbContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            List<Follower> follower = _context.Followers
                .Include(f => f.Product)
                .AsNoTracking()
                .ToList();

            List<Category>? categories = _context.Categories.Include(c => c.SubCategories).ThenInclude(c => c.SubSubCategories).ToList();

            ViewBag.Categories = categories;
            return View(follower);
        }
    }
}
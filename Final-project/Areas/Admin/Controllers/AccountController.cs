
using Final_project.Areas.Admin.Model;
using Final_project.Data;
using Final_project.Entities;
using Final_project.Utilities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MichaelDbContext _context;

        public AccountController(UserManager<User> userManager, MichaelDbContext context)
        {
            _userManager = userManager;

            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                ModelState.AddModelError("", "This username is already in use");
                return View(model);
            }

            userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                ModelState.AddModelError("", "This Email address is already in use");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name,
                SurName = model.SurName,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home", "Admin");
        }
    }
}

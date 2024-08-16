using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Data;
using Final_project.Entities;
using Final_project.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class FollowerController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public FollowerController(MichaelDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<FollowerViewModel> followers = _mapper.Map<List<FollowerViewModel>>(await _context.Followers
                .Include(x => x.Product)
                .OrderByDescending(p => p.Id).ToListAsync());

            foreach (var follower in followers)
            {
                follower.Products = await _context.Products.ToListAsync();
            }
            return View(followers);
        }

        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.AsEnumerable();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowerViewModel newFollowerViewModel)
        {
            ViewBag.Products = _context.Products.AsEnumerable();

            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                return View(newFollowerViewModel);
            }

            if (newFollowerViewModel.ImagePath is null)
            {
                ModelState.AddModelError("ImagePath", "ImagePath cannot be null");
                return View(newFollowerViewModel);
            }

            var fProduct = _context.Products.FirstOrDefault(x => x.Id == newFollowerViewModel.ProductId);

            if (fProduct == null)
            {
                newFollowerViewModel.Products = _context.Products.AsNoTracking().ToList();
                return View(newFollowerViewModel);
            }

            string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            if (!newFollowerViewModel.ImagePath.IsValidFile("image/"))
            {
                ModelState.AddModelError("", "Image is invalid!");
                return View(newFollowerViewModel);
            }

            newFollowerViewModel.Image = await newFollowerViewModel.ImagePath.CreateImage(imageFolderPath, "website-images");

            Follower newFollower = _mapper.Map<Follower>(newFollowerViewModel);

            newFollower.Product = fProduct;

            _context.Followers.Add(newFollower);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var followerToDelete = _context.Followers.FirstOrDefault(f => f.Id == id);

            if (followerToDelete == null)
            {
                return NotFound();
            }

            var followerViewModel = _mapper.Map<FollowerViewModel>(followerToDelete);

            return View(followerViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var followerToDelete = _context.Followers.FirstOrDefault(f => f.Id == id);

            if (followerToDelete == null)
            {
                return NotFound();
            }

            var imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images");

            var imagePath = Path.Combine(imageFolderPath, followerToDelete.Image);

            await FileUpload.DeleteImage(imagePath);

            _context.Followers.Remove(followerToDelete);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            var follower = _context.Followers
                                    .Include(f => f.Product)
                                    .FirstOrDefault(f => f.Id == id);

            if (follower == null)
            {
                return NotFound();
            }

            var followerViewModel = _mapper.Map<FollowerViewModel>(follower);
            followerViewModel.Products = _context.Products.ToList();

            return View(followerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(FollowerViewModel updatedFollowerViewModel)
        {
            if (!ModelState.IsValid)
            {
                updatedFollowerViewModel.Products = _context.Products.ToList();
                return View(updatedFollowerViewModel);
            }

            var existingFollower = _context.Followers
                                             .Include(f => f.Product)
                                             .FirstOrDefault(f => f.Id == updatedFollowerViewModel.Id);

            if (existingFollower == null)
            {
                return NotFound();
            }

            var selectedProduct = _context.Products.FirstOrDefault(p => p.Id == updatedFollowerViewModel.ProductId);
            if (selectedProduct == null)
            {
                ModelState.AddModelError("", "Selected product not found");
                updatedFollowerViewModel.Products = _context.Products.ToList();
                return View(updatedFollowerViewModel);
            }


            existingFollower.Name = updatedFollowerViewModel.Name;
            existingFollower.Product = selectedProduct;

            if (updatedFollowerViewModel.ImagePath != null && updatedFollowerViewModel.ImagePath.Length > 0)
            {
                string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                if (!updatedFollowerViewModel.ImagePath.IsValidFile("image/"))
                {
                    ModelState.AddModelError("", "Image is invalid!");
                    updatedFollowerViewModel.Products = _context.Products.ToList();
                    return View(updatedFollowerViewModel);
                }

                existingFollower.Image = await updatedFollowerViewModel.ImagePath.CreateImage(imageFolderPath, "website-images");
            }


            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}

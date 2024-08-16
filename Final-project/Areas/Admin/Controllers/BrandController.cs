using Final_project.Entities;
using Final_project.Data;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Final_project.Areas.Admin.Model;
using Microsoft.AspNetCore.Authorization;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class BrandController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IMapper _mapper;

        public BrandController(MichaelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            List<BrandViewModel> brandViewModels = _mapper.Map<List<BrandViewModel>>(_context.Brands.OrderByDescending(p => p.Id).ToList());
            return View(brandViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BrandViewModel newBrandViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicated = _context.Brands.Any(c => c.Name == newBrandViewModel.Name);
                if (isDuplicated)
                {
                    ModelState.AddModelError("", "You cannot duplicate value");
                }
                else
                {
                    Brand newBrand = _mapper.Map<Brand>(newBrandViewModel);
                    _context.Brands.Add(newBrand);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(newBrandViewModel);
        }

        public IActionResult Delete(int id)
        {
            Brand brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            BrandViewModel brandViewModel = _mapper.Map<BrandViewModel>(brand);
            return brandViewModel != null ? View(brandViewModel) : NotFound();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Brand brandToDelete = _context.Brands.FirstOrDefault(c => c.Id == id);
            if (brandToDelete != null)
            {
                _context.Brands.Remove(brandToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public IActionResult Update(int id)
        {
            Brand brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            BrandViewModel brandViewModel = _mapper.Map<BrandViewModel>(brand);
            return brandViewModel != null ? View(brandViewModel) : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, BrandViewModel editedViewModel)
        {
            if (id != editedViewModel.Id)
            {
                return BadRequest();
            }

            Brand brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            if (brand != null)
            {
                bool duplicate = _context.Brands.Any(c => c.Name == editedViewModel.Name && editedViewModel.Name != brand.Name);
                if (duplicate)
                {
                    ModelState.AddModelError("", "You cannot duplicate brand name");
                }
                else
                {
                    _mapper.Map(editedViewModel, brand);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(editedViewModel);
        }
    }

}


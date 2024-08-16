using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Data;
using Final_project.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(MichaelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<CategoryViewModel> categoryViewModels = _mapper.Map<List<CategoryViewModel>>(_context.Categories.OrderByDescending(p => p.Id).ToList());
            return View(categoryViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryViewModel newCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicated = _context.Categories.Any(c => c.Name == newCategoryViewModel.Name);
                if (isDuplicated)
                {
                    ModelState.AddModelError("", "You cannot duplicate value");
                }
                else
                {
                    Category newCategory = _mapper.Map<Category>(newCategoryViewModel);
                    _context.Categories.Add(newCategory);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(newCategoryViewModel);
        }

        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            CategoryViewModel categoryViewModel = _mapper.Map<CategoryViewModel>(category);
            return categoryViewModel != null ? View(categoryViewModel) : NotFound();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Category categoryToDelete = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (categoryToDelete != null)
            {
                _context.Categories.Remove(categoryToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public IActionResult Update(int id)
        {
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            CategoryViewModel categoryViewModel = _mapper.Map<CategoryViewModel>(category);
            return categoryViewModel != null ? View(categoryViewModel) : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, CategoryViewModel editedViewModel)
        {
            if (id != editedViewModel.Id)
            {
                return BadRequest();
            }

            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                bool duplicate = _context.Categories.Any(c => c.Name == editedViewModel.Name && editedViewModel.Name != category.Name);
                if (duplicate)
                {
                    ModelState.AddModelError("", "You cannot duplicate brand name");
                }
                else
                {
                    _mapper.Map(editedViewModel, category);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(editedViewModel);
        }
    }
}

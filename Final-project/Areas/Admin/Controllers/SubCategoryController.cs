using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Data;
using Final_project.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class SubCategoryController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IMapper _mapper;

        public SubCategoryController(MichaelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<SubCategoryViewModel> subCategories = _mapper.Map<List<SubCategoryViewModel>>(await _context.SubCategories
                .Include(x => x.Category)
                .OrderByDescending(p => p.Id).ToListAsync());

            foreach (var subCategory in subCategories)
            {
                subCategory.Categories = await _context.Categories.ToListAsync();
            }
            return View(subCategories);
        }


        public IActionResult Create()
        {
            var subCategoryViewModel = new SubCategoryViewModel
            {
                Categories = _context.Categories.ToList()
            };
            return View(subCategoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubCategoryViewModel newSubCategoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                return View(newSubCategoryViewModel);
            }

            var fCategory = _context.Categories.FirstOrDefault(x => x.Id == newSubCategoryViewModel.CategoryId);

            if (fCategory == null)
            {
                newSubCategoryViewModel.Categories = _context.Categories.AsNoTracking().ToList();
                return View(newSubCategoryViewModel);
            }
            SubCategory newCategory = _mapper.Map<SubCategory>(newSubCategoryViewModel);

            newCategory.Category = fCategory;

            _context.SubCategories.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            SubCategory? category = _context.SubCategories.Include(x => x.Category).FirstOrDefault(c => c.Id == id);
            SubCategoryViewModel categoryViewModel = _mapper.Map<SubCategoryViewModel>(category);

            categoryViewModel.Categories = _context.Categories.ToList();

            return categoryViewModel != null ? View(categoryViewModel) : NotFound();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            SubCategory categoryToDelete = _context.SubCategories.FirstOrDefault(c => c.Id == id);


            if (categoryToDelete != null)
            {
                _context.SubCategories.Remove(categoryToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }



        public IActionResult Update(int id)
        {
            var subCategory = _context.SubCategories.Include(s => s.Category).FirstOrDefault(s => s.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            var subCategoryViewModel = new SubCategoryViewModel
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                CategoryId = subCategory.Category.Id,
                Categories = _context.Categories.ToList()
            };

            return View(subCategoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, SubCategoryViewModel updatedSubCategoryViewModel)
        {
            if (id != updatedSubCategoryViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                updatedSubCategoryViewModel.Categories = _context.Categories.ToList();
                return View(updatedSubCategoryViewModel);
            }


            var fCategory = _context.Categories.FirstOrDefault(x => x.Id == updatedSubCategoryViewModel.CategoryId);

            if (fCategory == null)
            {
                updatedSubCategoryViewModel.Categories = _context.Categories.AsNoTracking().ToList();
                return View(updatedSubCategoryViewModel);
            }

            var subCategoryToUpdate = _context.SubCategories.FirstOrDefault(s => s.Id == id);

            if (subCategoryToUpdate == null)
            {
                return NotFound();
            }

            subCategoryToUpdate.Name = updatedSubCategoryViewModel.Name;
            subCategoryToUpdate.Category = fCategory;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}

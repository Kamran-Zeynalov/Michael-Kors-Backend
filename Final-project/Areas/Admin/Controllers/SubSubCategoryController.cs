using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Data;
using Final_project.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class SubSubCategoryController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IMapper _mapper;

        public SubSubCategoryController(MichaelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<SubSubCategoryViewModel> subCategories = _mapper.Map<List<SubSubCategoryViewModel>>(await _context.SubSubCategories.Include(x => x.SubCategory)
                .OrderByDescending(p => p.Id).ToListAsync());

            foreach (var subCategory in subCategories)
            {
                subCategory.SubCategories = await _context.SubCategories.Include(s => s.Category).ToListAsync();
            }
            return View(subCategories);
        }


        public IActionResult Create()
        {
            var subCategoryViewModel = new SubSubCategoryViewModel
            {
                SubCategories = _context.SubCategories.Include(p => p.Category).ToList()
            };
            return View(subCategoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubSubCategoryViewModel newSubCategoryViewModel)
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

            var fCategory = _context.SubCategories.FirstOrDefault(x => x.Id == newSubCategoryViewModel.SubCategoryId);


            if (fCategory == null)
            {
                newSubCategoryViewModel.SubCategories = _context.SubCategories.Include(s => s.Category).AsNoTracking().ToList();
                return View(newSubCategoryViewModel);
            }

            SubSubCategory newCategory = _mapper.Map<SubSubCategory>(newSubCategoryViewModel);

            newCategory.SubCategory = fCategory;

            _context.SubSubCategories.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            SubSubCategory? category = _context.SubSubCategories.Include(x => x.SubCategory).FirstOrDefault(c => c.Id == id);
            SubSubCategoryViewModel categoryViewModel = _mapper.Map<SubSubCategoryViewModel>(category);


            categoryViewModel.SubCategories = _context.SubCategories.ToList();

            return categoryViewModel != null ? View(categoryViewModel) : NotFound();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            SubSubCategory? categoryToDelete = _context.SubSubCategories.FirstOrDefault(c => c.Id == id);


            if (categoryToDelete != null)
            {
                _context.SubSubCategories.Remove(categoryToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }



        public IActionResult Update(int id)
        {
            var subCategory = _context.SubSubCategories.Include(s => s.SubCategory).ThenInclude(s => s.Category).FirstOrDefault(s => s.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            var subCategoryViewModel = new SubSubCategoryViewModel
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                SubCategoryId = subCategory.SubCategory.Id,
                SubCategories = _context.SubCategories.Include(s => s.Category).ToList()
            };

            return View(subCategoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, SubSubCategoryViewModel updatedSubCategoryViewModel)
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
                updatedSubCategoryViewModel.SubCategories = _context.SubCategories.Include(s => s.Category).ToList();
                return View(updatedSubCategoryViewModel);
            }


            var fCategory = _context.SubCategories.FirstOrDefault(x => x.Id == updatedSubCategoryViewModel.SubCategoryId);

            if (fCategory == null)
            {
                updatedSubCategoryViewModel.SubCategories = _context.SubCategories.Include(s => s.Category).AsNoTracking().ToList();
                return View(updatedSubCategoryViewModel);
            }

            var subCategoryToUpdate = _context.SubSubCategories.FirstOrDefault(s => s.Id == id);

            if (subCategoryToUpdate == null)
            {
                return NotFound();
            }

            subCategoryToUpdate.Name = updatedSubCategoryViewModel.Name;
            subCategoryToUpdate.SubCategory = fCategory;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}

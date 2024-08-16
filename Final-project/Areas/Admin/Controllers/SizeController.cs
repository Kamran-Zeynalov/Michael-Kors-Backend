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
    public class SizeController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IMapper _mapper;

        public SizeController(MichaelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<SizeViewModel> sizeViewModels = _mapper.Map<List<SizeViewModel>>(_context.Sizes.OrderByDescending(p => p.Id).ToList());
            return View(sizeViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SizeViewModel newSizeViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicated = _context.Sizes.Any(c => c.Name == newSizeViewModel.Name);
                if (isDuplicated)
                {
                    ModelState.AddModelError("", "You cannot duplicate value");
                }
                else
                {
                    Size newSize = _mapper.Map<Size>(newSizeViewModel);
                    _context.Sizes.Add(newSize);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(newSizeViewModel);
        }

        public IActionResult Delete(int id)
        {
            Size size = _context.Sizes.FirstOrDefault(c => c.Id == id);
            SizeViewModel sizeViewModel = _mapper.Map<SizeViewModel>(size);
            return sizeViewModel != null ? View(sizeViewModel) : NotFound();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Size sizeToDelete = _context.Sizes.FirstOrDefault(c => c.Id == id);
            if (sizeToDelete != null)
            {
                _context.Sizes.Remove(sizeToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public IActionResult Update(int id)
        {
            Size size = _context.Sizes.FirstOrDefault(c => c.Id == id);
            SizeViewModel sizeViewModel = _mapper.Map<SizeViewModel>(size);
            return sizeViewModel != null ? View(sizeViewModel) : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, SizeViewModel editedViewModel)
        {
            if (id != editedViewModel.Id)
            {
                return BadRequest();
            }

            Size size = _context.Sizes.FirstOrDefault(c => c.Id == id);
            if (size != null)
            {
                bool duplicate = _context.Sizes.Any(c => c.Name == editedViewModel.Name && editedViewModel.Name != size.Name);
                if (duplicate)
                {
                    ModelState.AddModelError("", "You cannot duplicate size name");
                }
                else
                {
                    _mapper.Map(editedViewModel, size);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(editedViewModel);
        }
    }
}

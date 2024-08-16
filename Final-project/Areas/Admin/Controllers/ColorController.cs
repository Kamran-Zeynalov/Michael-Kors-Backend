using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Data;
using Final_project.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class ColorController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IMapper _mapper;

        public ColorController(MichaelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<ColorViewModel> colorViewModels = _mapper.Map<List<ColorViewModel>>(_context.Colors.OrderByDescending(p => p.Id).ToList());
            return View(colorViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ColorViewModel newColorViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicated = _context.Colors.Any(c => c.Name == newColorViewModel.Name);
                if (isDuplicated)
                {
                    ModelState.AddModelError("", "You cannot duplicate value");
                }
                else
                {
                    Color newColor = _mapper.Map<Color>(newColorViewModel);
                    _context.Colors.Add(newColor);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(newColorViewModel);
        }

        public IActionResult Delete(int id)
        {
            Color color = _context.Colors.FirstOrDefault(c => c.Id == id);
            ColorViewModel colorViewModel = _mapper.Map<ColorViewModel>(color);
            return colorViewModel != null ? View(colorViewModel) : NotFound();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Color colorToDelete = _context.Colors.FirstOrDefault(c => c.Id == id);
            if (colorToDelete != null)
            {
                _context.Colors.Remove(colorToDelete);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public IActionResult Update(int id)
        {
            Color color = _context.Colors.FirstOrDefault(c => c.Id == id);
            ColorViewModel colorViewModel = _mapper.Map<ColorViewModel>(color);
            return colorViewModel != null ? View(colorViewModel) : NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, ColorViewModel editedViewModel)
        {
            if (id != editedViewModel.Id)
            {
                return BadRequest();
            }

            Color color = _context.Colors.FirstOrDefault(c => c.Id == id);
            if (color != null)
            {
                bool duplicate = _context.Colors.Any(c => c.Name == editedViewModel.Name && editedViewModel.Name != color.Name);
                if (duplicate)
                {
                    ModelState.AddModelError("", "You cannot duplicate color name");
                }
                else
                {
                    _mapper.Map(editedViewModel, color);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(editedViewModel);
        }
    }
}

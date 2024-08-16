using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Data;
using Final_project.Entities;
using Final_project.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class ProductController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ProductController(MichaelDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Products.Count() / 4);
            ViewBag.CurrentPage = page;

            IEnumerable<Product> model = _context.Products.Include(p => p.Images)
                .Include(p => p.SubSubCategory)
                    .ThenInclude(p => p.SubCategory)
                    .ThenInclude(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
                .OrderByDescending(p => p.Id)
                 .AsNoTracking().Skip((page - 1) * 4).Take(4).AsEnumerable();

            return View(model);
        }


        public IActionResult Create()
        {
            var productViewModel = new ProductViewModel
            {
                SubSubCategories = _context.SubSubCategories.Include(s => s.SubCategory).ThenInclude(s => s.Category).ToList(),
                Brands = _context.Brands.ToList(),
                Colors = _context.Colors.ToList(),
                Sizes = _context.Sizes.ToList(),
            };
            return View(productViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel newProduct)
        {
            ViewBag.Categories = _context.SubSubCategories.Include(s => s.SubCategory).ThenInclude(s => s.Category).AsEnumerable();
            ViewBag.Brands = _context.Brands.AsEnumerable();
            TempData["InvalidImages"] = string.Empty;


            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                return View(newProduct);
            }

            Product product = _mapper.Map<Product>(newProduct);

            var fCategory = _context.SubSubCategories.FirstOrDefault(x => x.Id == newProduct.SubSubCategoryId);
            var fBrand = _context.Brands.FirstOrDefault(x => x.Id == newProduct.BrandId);
            var fColor = _context.Colors.FirstOrDefault(x => x.Id == newProduct.ColorId);
            var fSize = _context.Sizes.FirstOrDefault(x => x.Id == newProduct.SizeId);


            if (fCategory == null)
            {
                newProduct.SubSubCategories = _context.SubSubCategories.Include(s => s.SubCategory).ThenInclude(s => s.Category).AsNoTracking().ToList();
                return View(newProduct);
            }
            product.SubSubCategory = fCategory;

            if (fBrand == null)
            {
                newProduct.Brands = _context.Brands.AsNoTracking().ToList();
                return View(newProduct);
            }
            product.Brand = fBrand;

            string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
            foreach (var image in newProduct.ImageFiles)
            {
                if (!image.IsValidFile("image/"))
                {
                    TempData["InvalidImages"] += image.FileName;
                    continue;
                }
                Image productImage = new()
                {

                    Name = await image.CreateImage(imageFolderPath, "website-images")
                };
                product.Images.Add(productImage);
            }

            string[] colorSizeCounts = newProduct.ColorSizeCount.Split(',');
            foreach (string colorSizeCount in colorSizeCounts)
            {
                string[] datas = colorSizeCount.Split('-');
                ProductSizeColor productSizeColor = new()
                {
                    Name = "Random",
                    ColorId = int.Parse(datas[0]),
                    SizeId = int.Parse(datas[1]),
                    Count = int.Parse(datas[2])
                };
                product.ProductSizeColors.Add(productSizeColor);
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var imagesFolderPath = Path.Combine(_env.WebRootPath, "assets", "images", "website-images");

            var imageFileNames = await _context.Images
                .Where(i => i.Id == product.Id)
                .Select(i => i.Name)
                .ToListAsync();

            foreach (var imageFileName in imageFileNames)
            {
                var imagePath = Path.Combine(imagesFolderPath, imageFileName);

                await FileUpload.DeleteImage(imagePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            Product product = _context.Products
                .Include(p => p.Images)
                .Include(p => p.SubSubCategory).ThenInclude(ssc => ssc.SubCategory)
                .ThenInclude(sc => sc.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductSizeColors).ThenInclude(psc => psc.Color)
                .Include(p => p.ProductSizeColors).ThenInclude(psc => psc.Size)
                .FirstOrDefault(x => x.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Update(int? id)
        {
            if (id is null) return BadRequest();
            ProductUpdateViewModel? model = EditedProduct(id);
            if (model is null) return BadRequest();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductUpdateViewModel edited)
        {
            ProductUpdateViewModel? model = EditedProduct(id);
            Product? product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return BadRequest();

            var fCategory = _context.SubSubCategories.FirstOrDefault(x => x.Id == edited.SubSubCategoryId);
            if (fCategory == null)
            {
                ViewData["Categories"] = _context.SubSubCategories.Include(s => s.SubCategory).ThenInclude(s => s.Category).AsNoTracking().ToList();
                return View(edited);
            }
            product.SubSubCategory = fCategory;

            var fBrand = _context.Brands.FirstOrDefault(x => x.Id == edited.BrandId);
            if (fBrand == null)
            {
                ViewData["Brands"] = _context.Brands.AsNoTracking().ToList();
                return View(edited);
            }
            product.Brand = fBrand;


            product.ProductSizeColors.Clear();
            string[] colorSizeCounts = edited.ColorSizeCount.Split(',');
            foreach (string colorSizeCount in colorSizeCounts)
            {
                string[] datas = colorSizeCount.Split('-');
                ProductSizeColor productSizeColor = new()
                {
                    Name = "Random",
                    ColorId = int.Parse(datas[0]),
                    SizeId = int.Parse(datas[1]),
                    Count = int.Parse(datas[2])
                };
                product.ProductSizeColors.Add(productSizeColor);
            }


            if (edited.DeletedImageIds != null && edited.DeletedImageIds.Any())
            {
                if (edited.DeletedImageIds.Count == product.Images.Count)
                {
                    edited.Images = product.Images.ToList();
                    edited.Colors = _context.Colors.AsNoTracking().ToList();
                    edited.Sizes = _context.Sizes.AsNoTracking().ToList();
                    edited.SubSubCategories = _context.SubSubCategories.Include(s => s.SubCategory).ThenInclude(s => s.Category).AsNoTracking().ToList();
                    edited.Brands = _context.Brands.AsNoTracking().ToList();
                    ModelState.AddModelError("ImageFiles", "Image can not be null");
                    return View(edited);
                }
                foreach (var deletedImageId in edited.DeletedImageIds)
                {
                    Image? imageToDelete = _context.Images.FirstOrDefault(i => i.Id == deletedImageId);
                    if (imageToDelete != null)
                    {
                        var imagePath = Path.Combine("assets", "images", "website-images", imageToDelete.Name);
                        FileUpload.DeleteFile(imageToDelete.Name, imagePath);

                        _context.Images.Remove(imageToDelete);
                    }
                }

            }
            if (edited.ImageFiles is not null)
            {
                string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");
                foreach (var image in edited.ImageFiles)
                {
                    if (!image.IsValidFile("image/"))
                    {
                        TempData["InvalidImages"] += image.FileName;
                        continue;
                    }
                    Image productImage = new()
                    {
                        Name = await image.CreateImage(imageFolderPath, "website-images")
                    };
                    product.Images.Add(productImage);
                }
            }

            product.Name = edited.Name;
            product.Price = edited.Price;
            product.DiscountPrice = edited.DiscountPrice;
            product.Count = edited.Count;



            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private ProductUpdateViewModel? EditedProduct(int? id)
        {
            ProductUpdateViewModel? model = _context.Products
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                .Include(p => p.Images).Include(p => p.SubSubCategory).ThenInclude(s => s.SubCategory).ThenInclude(s => s.Category)
                .Include(p => p.Brand)
                .Select(p =>
                   new ProductUpdateViewModel()
                   {
                       Id = p.Id,
                       Name = p.Name,
                       Price = p.Price,
                       DiscountPrice = p.DiscountPrice,
                       Images = p.Images.ToList(),
                       Count = p.Count,
                       SubSubCategories = _context.SubSubCategories.Include(s => s.SubCategory).ThenInclude(s => s.Category).ToList(),
                       ColorSizeCount = GetColorSizeCountForProduct(id.Value),
                       Brands = _context.Brands.ToList(),
                       Colors = _context.Colors.ToList(),
                       Sizes = _context.Sizes.ToList(),
                       SubSubCategoryId = p.SubSubCategory.Id,
                       BrandId = p.Brand.Id,
                       ColorId = p.ProductSizeColors.Select(p => p.ColorId).FirstOrDefault(),
                       SizeId = p.ProductSizeColors.Select(p => p.SizeId).FirstOrDefault()
                   }
                ).FirstOrDefault(x => x.Id == id);
            return model;
        }

        private string GetColorSizeCountForProduct(int productId)
        {
            var product = _context.Products.Include(p => p.ProductSizeColors).FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                var colorSizeCount = string.Join(",", product.ProductSizeColors.Select(psc => $"{psc.ColorId}-{psc.SizeId}-{psc.Count}"));
                return colorSizeCount;
            }
            return string.Empty;
        }

        public async Task<IActionResult> SearchResult(string search, int page = 1)
        {
            IQueryable<Product> proList = _context.Products.Include(p => p.Images)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size);

            if (!string.IsNullOrEmpty(search))
            {
                proList = proList.Where(c => c.Name.ToUpper().Contains(search.ToUpper()));
            }

            proList = proList.OrderByDescending(x => x.Id);

            return PartialView("_ProductPartial", await proList.ToListAsync());
        }

    }
}

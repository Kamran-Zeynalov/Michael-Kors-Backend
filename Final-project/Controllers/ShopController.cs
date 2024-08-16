using Final_project.Data;
using Final_project.Entities;
using Final_project.Models;
using Final_project.Services.Implementations;
using Final_project.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace Final_project.Controllers
{
    public class ShopController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IHttpContextAccessor _http;
        private readonly IWishlistService _wishlistService;

        public ShopController(MichaelDbContext context, IBasketService basketService, IHttpContextAccessor http, IWishlistService wishlistService)
        {
            _context = context;
            _basketService = basketService;
            _http = http;
            _wishlistService = wishlistService;
        }
        public async Task<IActionResult> Index(int categoryId, int subCategoryId, int page = 1)
        {
            ViewBag.Categories = await _context.SubSubCategories.ToListAsync();
            ViewBag.Brands = await _context.Brands.ToListAsync();
            ViewBag.Sizes = await _context.ProductSizeColors.Include(p => p.Color).ToListAsync();
            ViewBag.Colors = await _context.ProductSizeColors.Include(p => p.Size).ToListAsync();
            List<Product> products = await GetProductIndex(page, categoryId, subCategoryId);
            return View(products);
        }

        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (id is null) return BadRequest();
            Product? products = await GetProductDetail(id);
            ViewBag.Products = await _context.Products
                .Include(p => p.SubSubCategory).ThenInclude(p => p.SubCategory)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                .Include(p => p.Images)
                .Include(p => p.Brand)
                .ToListAsync();
            if (products is null) return BadRequest();
            return View(products);
        }

        public async Task<IActionResult> AddBasket(int id, int colorId, int sizeId, int? count)
        {
            if (_http.HttpContext.User.Identity.IsAuthenticated)
            {
                TempData["Basket"] = null;
                TempData["Stock"] = null;
                ViewBag.CountPro = _context.ProductSizeColors.Where(psc => psc.ProductId == id && psc.ColorId == colorId && psc.SizeId == sizeId).FirstOrDefault()?.Count;
                if (count > ViewBag.CountPro)
                {
                    TempData["Stock"] = true;
                    return RedirectToAction("ProductDetail", "Shop", new { id = id });
                }
                await _basketService.AddBasket(id, colorId, sizeId, count);
                TempData["Basket"] = true;
                return RedirectToAction("ProductDetail", "Shop", new { id = id });
            }
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> RemoveBasket(int id, int colorId, int sizeId)
        {
            TempData["Basket"] = false;
            await _basketService.RemoveBasketItem(id, colorId, sizeId);
            TempData["Basket"] = true;
            return RedirectToAction("Cart", "Basket");
        }

        public async Task<IActionResult> AddWislist(int id, int categoryId, int subCategoryId)
        {
            TempData["Favorite"] = false;
            if (_http.HttpContext.User.Identity.IsAuthenticated)
            {
                await _wishlistService.AddWishlist(id);
                TempData["Favorite"] = true;
                return RedirectToAction("Index", "Shop", new { categoryId = categoryId, subCategoryId = subCategoryId });
            }
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> RemoveWislist(int id)
        {
            TempData["Favorite"] = false;
            await _wishlistService.RemoveWishlistItem(id);
            TempData["Favorite"] = true;
            return RedirectToAction("Favorite", "Basket");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (User.Identity.IsAuthenticated)
            {
                var newComment = new Comment
                {
                    UserName = User.Identity.Name,
                    Rating = model.Rating,
                    ReviewText = model.ReviewText,
                    ProductId = model.ProductId,
                    AgreeToTerms = model.AgreeToTerms,
                    Time = DateTime.Now
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                return RedirectToAction("ProductDetail", new { id = model.ProductId });
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Comment not found" });
            }
        }




        //Extension methods
        public async Task<Product>? GetProductDetail(int? id)
        {
            Product? model = await _context.Products
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                .Include(p => p.Images).Include(p => p.SubSubCategory)
                .Include(p => p.Brand)
                .Include(p => p.Comments.OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id);
            return model;
        }

        public async Task<List<Product>>? GetProductIndex(int page, int categoryId, int subCategoryId)
        {
            ViewBag.TotalPage = Math.Ceiling((double)_context.Products
                .Include(p => p.SubSubCategory).ThenInclude(c => c.SubCategory)
                 .Where(p => p.SubSubCategory.Id == categoryId ||
                p.SubSubCategory.SubCategory.Id == subCategoryId).Count() / 4);
            ViewBag.CurrentPage = page;

            List<Product>? model = await _context.Products
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
                .Include(p => p.Images)
                .Include(p => p.Brand)
                .Include(p => p.SubSubCategory).ThenInclude(c => c.SubCategory).OrderByDescending(p => p.Id)
                .Where(p => p.SubSubCategory.Id == categoryId ||
                p.SubSubCategory.SubCategory.Id == subCategoryId)
                .Skip((page - 1) * 4).Take(4)
                .ToListAsync();
            return model;
        }

        public async Task<List<Product>>? GetSortedProductIndex(int categoryId, int subCategoryId)
        {
            List<Product>? model = await _context.Products
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
                .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
                .Include(p => p.Images)
                .Include(p => p.Brand)
                .Include(p => p.SubSubCategory).ThenInclude(c => c.SubCategory).OrderByDescending(p => p.Id)
                .Where(p => p.SubSubCategory.Id == categoryId ||
                p.SubSubCategory.SubCategory.Id == subCategoryId)
                .ToListAsync();
            return model;
        }

        public async Task<IActionResult> Sorted(int brandid, int sizeid, int colorid, int categoryId, int subCategoryId, int priceRange, int page = 1)
        {
            List<Product> products = await GetSortedProductIndex(categoryId, subCategoryId);

            List<Product> sortProducts = products
                .Where(x => (brandid == 0 || x.Brand.Id == brandid) &&
                            (sizeid == 0 || x.ProductSizeColors.Any(p => p.SizeId == sizeid) || x.ProductSizeColors.Count == 0) &&
                            (colorid == 0 || x.ProductSizeColors.Any(p => p.ColorId == colorid) || x.ProductSizeColors.Count == 0))
                .OrderByDescending(x => x.Id)
                .ToList();

            ViewBag.CategoryId = categoryId;
            ViewBag.SubCategoryId = subCategoryId;
            ViewBag.Page = page;
            ViewBag.Count = sortProducts.Count;

            var productInfo = new { Count = ViewBag.Count };
            var serializedProductInfo = JsonConvert.SerializeObject(productInfo);
            ViewData["ProductInfo"] = serializedProductInfo;


            switch (priceRange)
            {
                case 1:
                    sortProducts = sortProducts.Where(p =>
                        (GetDisplayedPrice(p) < 100)
                    ).ToList();
                    break;
                case 2:
                    sortProducts = sortProducts.Where(p =>
                        (GetDisplayedPrice(p) >= 100 && GetDisplayedPrice(p) < 200)
                    ).ToList();
                    break;
                case 3:
                    sortProducts = sortProducts.Where(p =>
                        (GetDisplayedPrice(p) >= 200 && GetDisplayedPrice(p) < 300)
                    ).ToList();
                    break;
                case 4:
                    sortProducts = sortProducts.Where(p =>
                        (GetDisplayedPrice(p) >= 300)
                    ).ToList();
                    break;
                default:
                    break;
            }

            decimal GetDisplayedPrice(Product p)
            {
                if (p.DiscountPrice != 0)
                {
                    return p.DiscountPrice;
                }
                else
                {
                    return p.Price;
                }
            }

            return PartialView("_ProductBasketPartial", sortProducts);
        }

        public async Task<IActionResult> SortedProduct(int[] brandid, int[] sizeid, int[] colorid, int categoryId, int subCategoryId, int[] priceRange, int page = 1)
        {
            List<Product> products = await GetSortedProductIndex(categoryId, subCategoryId);

            if (brandid.Length > 0)
            {
                products = products.Where(p => brandid.Contains(p.Brand.Id)).ToList();
            }

            if (sizeid.Length > 0)
            {
                products = products.Where(p => p.ProductSizeColors.Any(x => sizeid.Contains(x.Size.Id))).ToList();
            }
            if (colorid.Length > 0)
            {
                products = products.Where(p => p.ProductSizeColors.Any(x => colorid.Contains(x.Color.Id))).ToList();
            }

            ViewBag.CategoryId = categoryId;
            ViewBag.SubCategoryId = subCategoryId;
            ViewBag.Page = page;
            ViewBag.Count = products.Count;

            var productInfo = new { Count = ViewBag.Count };
            var serializedProductInfo = JsonConvert.SerializeObject(productInfo);
            ViewData["ProductInfo"] = serializedProductInfo;

            foreach (var price in priceRange)
            {

                switch (price)
                {
                    case 1:
                        products = products.Where(p =>
                            (GetDisplayedPrice(p) < 100)
                        ).ToList();
                        break;
                    case 2:
                        products = products.Where(p =>
                            (GetDisplayedPrice(p) >= 100 && GetDisplayedPrice(p) < 200)
                        ).ToList();
                        break;
                    case 3:
                        products = products.Where(p =>
                            (GetDisplayedPrice(p) >= 200 && GetDisplayedPrice(p) < 300)
                        ).ToList();
                        break;
                    case 4:
                        products = products.Where(p =>
                            (GetDisplayedPrice(p) >= 300)
                        ).ToList();
                        break;
                    default:
                        break;
                }
            }
            decimal GetDisplayedPrice(Product p)
            {
                if (p.DiscountPrice != 0)
                {
                    return p.DiscountPrice;
                }
                else
                {
                    return p.Price;
                }
            }

            return PartialView("_ProductBasketPartial", products);
        }

        public async Task<IActionResult> SearchResult(string search, int page = 1)
        {
            List<Product> proList = await _context.Products.Include(p => p.Images)
                .Include(p => p.Brand)
                .Include(p => p.ProductSizeColors).ThenInclude(psc => psc.Color)
                .Include(p => p.SubSubCategory).ThenInclude(p => p.SubCategory)
                .ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                proList = proList.Where(c => c.Name.ToUpper().Contains(search.ToUpper())).ToList();
            }

            proList = proList.OrderByDescending(x => x.Id).ToList();

            return PartialView("_ProductBasketPartial", proList.ToList());
        }
    }
}

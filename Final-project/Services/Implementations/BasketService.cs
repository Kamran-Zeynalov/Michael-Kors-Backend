using Final_project.Data;
using Final_project.Entities;
using Final_project.Exceptions;
using Final_project.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Final_project.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly MichaelDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<User> _userManager;

        public BasketService(MichaelDbContext context, IHttpContextAccessor http, UserManager<User> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }
        public async Task AddBasket(int id, int colorId, int sizeId, int? count)
        {
            Product? product = await _context.Products
               .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
               .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
               .Include(p => p.Images)
               .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) throw new ItemNotFoundException("Product Not found");


            User appUser = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);

            Basket? basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .FirstOrDefaultAsync(b => b.UserId == appUser.Id);
            if (basket != null)
            {
                BasketItem basketItem = basket.BasketItems.FirstOrDefault(x => x.ProductId == id && x.ColorId == colorId && x.SizeId == sizeId);

                if (basketItem != null)
                {
                    basketItem.Count += count ?? 1;
                    int totalProductCount = product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId).Sum(p => p.Count);

                    totalProductCount -= count ?? 1;

                    int totalCountAfterUpdate = totalProductCount / product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId).Count();
                    foreach (var sizeColor in product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId))
                    {
                        sizeColor.Count = totalCountAfterUpdate;
                    }
                    _context.Products.Update(product);
                    _context.BasketItems.Update(basketItem);
                }
                else
                {
                    basketItem = new BasketItem
                    {
                        Count = count ?? 1,
                        Basket = basket,
                        ProductId = id,
                        Product = product,
                        ColorId = product.ProductSizeColors.FirstOrDefault(p => p.ColorId == colorId).Color.Id,
                        SizeId = product.ProductSizeColors.FirstOrDefault(p => p.SizeId == sizeId).Size.Id
                    };

                    int totalProductCount = product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId).Sum(p => p.Count);

                    totalProductCount -= count ?? 1;

                    int totalCountAfterUpdate = totalProductCount / product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId).Count();
                    foreach (var sizeColor in product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId))
                    {
                        sizeColor.Count = totalCountAfterUpdate;
                    }
                    _context.Products.Update(product);
                    await _context.BasketItems.AddAsync(basketItem);
                }
            }
            else
            {
                basket = new Basket { UserId = appUser.Id };
                await _context.Baskets.AddAsync(basket);
                var basketItem = new BasketItem
                {
                    Count = count ?? 1,
                    Basket = basket,
                    ProductId = id,
                    ColorId = product.ProductSizeColors.FirstOrDefault(p => p.ColorId == colorId).Color.Id,
                    SizeId = product.ProductSizeColors.FirstOrDefault(p => p.SizeId == sizeId).Size.Id

                };

                int totalProductCount = product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId).Sum(p => p.Count);

                totalProductCount -= count ?? 1;

                int totalCountAfterUpdate = totalProductCount / product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId).Count();
                foreach (var sizeColor in product.ProductSizeColors.Where(p => p.ColorId == colorId && p.SizeId == sizeId))
                {
                    sizeColor.Count = totalCountAfterUpdate;
                }
                _context.Products.Update(product);
                await _context.AddAsync(basketItem);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<Basket> GetBasket()
        {
            Basket model = new();

            User appUser = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);
            var basketquery = await _context.Baskets
                                 .Include(x => x.BasketItems)
                                     .ThenInclude(x => x.Product)
                                         .ThenInclude(p => p.Images)
                                 .Include(x => x.BasketItems)
                                     .ThenInclude(x => x.Product)
                                         .ThenInclude(p => p.ProductSizeColors)
                                             .ThenInclude(p => p.Color)
                                 .Include(x => x.BasketItems)
                                     .ThenInclude(x => x.Product)
                                         .ThenInclude(p => p.ProductSizeColors)
                                             .ThenInclude(p => p.Size)
                                 .FirstOrDefaultAsync();

            if (basketquery != null)
            {
                model.BasketItems = basketquery.BasketItems.Select(bs => new BasketItem
                {
                    Count = bs.Count,
                    Id = bs.ProductId,
                    Product = bs.Product,
                    ColorId = bs.ColorId,
                    SizeId = bs.SizeId,
                }).ToList();

                decimal totalPrice = basketquery.BasketItems.Sum(bs =>
                {
                    if (bs.Product.DiscountPrice != 0)
                    {
                        decimal price = bs.Product.DiscountPrice;
                        return bs.Count * price;
                    }
                    else
                    {
                        decimal price = bs.Product.Price;
                        return bs.Count * price;
                    }
                });

                model.TotalPrice = totalPrice;
            }
            return model;
        }
        public async Task RemoveBasketItem(int id, int colorId, int sizeId)
        {
            Product? product = await _context.Products
             .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
             .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
             .Include(p => p.Images)
             .FirstOrDefaultAsync(p => p.Id == id);

            User appUser = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);
            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.ProductId == id && x.Basket.UserId == appUser.Id);
            if (basketItem != null)
            {
                foreach (var sizeColor in basketItem.Product.ProductSizeColors.Where(p => p.Color.Id == colorId && p.Size.Id == sizeId))
                {
                    sizeColor.Count += basketItem.Count;
                }
                _context.Products.Update(product);
                _context.BasketItems.Remove(basketItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}

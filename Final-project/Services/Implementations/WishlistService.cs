using Final_project.Data;
using Final_project.Entities;
using Final_project.Exceptions;
using Final_project.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly MichaelDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<User> _userManager;

        public WishlistService(MichaelDbContext context, IHttpContextAccessor http, UserManager<User> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }
        public async Task AddWishlist(int id)
        {
            Product? product = await _context.Products
             .Include(p => p.ProductSizeColors).ThenInclude(p => p.Color)
             .Include(p => p.ProductSizeColors).ThenInclude(p => p.Size)
             .Include(p => p.Images)
             .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) throw new ItemNotFoundException("Product Not found");

            User appUser = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);


            Wishlist? wishlist = await _context.Wishlists
                   .Include(b => b.WishlistItems)
                   .FirstOrDefaultAsync(b => b.UserId == appUser.Id);

            if (wishlist != null)
            {
                WishlistItem? wishlistItem = wishlist.WishlistItems.FirstOrDefault(x => x.ProductId == id);

                if (wishlistItem != null)
                {
                    _context.WishlistItems.Update(wishlistItem);
                }
                else
                {
                    wishlistItem = new WishlistItem
                    {
                        Wishlist = wishlist,
                        ProductId = id,
                        Product = product,
                    };
                    await _context.WishlistItems.AddAsync(wishlistItem);
                }
            }
            else
            {
                wishlist = new Wishlist { UserId = appUser.Id };
                await _context.Wishlists.AddAsync(wishlist);
                var wishlistItem = new WishlistItem
                {
                    Wishlist = wishlist,
                    ProductId = id,
                };
                await _context.AddAsync(wishlistItem);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Wishlist> GetWishlist()
        {
            Wishlist model = new();

            User appUser = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);
            var wislistquery = await _context.Wishlists
                                 .Include(x => x.WishlistItems)
                                     .ThenInclude(x => x.Product)
                                         .ThenInclude(p => p.Images)
                                 .FirstOrDefaultAsync();

            if (wislistquery != null)
            {
                model.WishlistItems = wislistquery.WishlistItems.Select(bs => new WishlistItem
                {
                    Id = bs.ProductId,
                    Product = bs.Product,
                }).ToList();
            }
            return model;
        }

        public async Task RemoveWishlistItem(int id)
        {
            User appUser = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);
            var wislistItem = await _context.WishlistItems.FirstOrDefaultAsync(x => x.ProductId == id && x.Wishlist.UserId == appUser.Id);
            if (wislistItem != null)
            {
                _context.WishlistItems.Remove(wislistItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}

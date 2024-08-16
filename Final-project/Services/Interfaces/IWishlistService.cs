using Final_project.Entities;

namespace Final_project.Services.Interfaces
{
    public interface IWishlistService
    {
        Task AddWishlist(int id);
        Task<Wishlist> GetWishlist();
        Task RemoveWishlistItem(int id);
    }
}

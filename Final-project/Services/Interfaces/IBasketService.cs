using Final_project.Entities;

namespace Final_project.Services.Interfaces
{
    public interface IBasketService
    {
        Task AddBasket(int id, int colorId, int sizeId, int? count);
        Task<Basket> GetBasket();
        Task RemoveBasketItem(int id, int colorId, int sizeId);
    }
}

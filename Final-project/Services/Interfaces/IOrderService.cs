using Final_project.Entities;
using Final_project.Models;
using Final_project.Response;

namespace Final_project.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CommonResponse> CreateAsync(OrderViewModel order);
        List<Order> GetAll();
        Task<Order> Get(int id);
        Task Accept(int id);
        Task Reject(int id);
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}

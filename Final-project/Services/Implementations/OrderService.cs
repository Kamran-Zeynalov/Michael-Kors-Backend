using Final_project.Data;
using Final_project.Entities;
using Final_project.Models;
using Final_project.Response;
using Final_project.Services.Interfaces;
using Final_project.Utilities.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly MichaelDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;

        public OrderService(MichaelDbContext context, IHttpContextAccessor http, IWebHostEnvironment env, UserManager<User> userManager)
        {
            _context = context;
            _http = http;
            _env = env;
            _userManager = userManager;
        }

        public async Task Accept(int id)
        {
            Order? order = await _context.Orders
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            User? user = await _context.Users.FirstOrDefaultAsync(p => p.Id == order.User.Id);
            order.Status = true;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<CommonResponse> CreateAsync(OrderViewModel order)
        {
            CommonResponse commonResponse = new CommonResponse();
            commonResponse.StatusCode = 200;

            if (order.ReceptImageFile is null)
            {
                commonResponse.StatusCode = 400;
                commonResponse.Message = "Upload a picture of the check";
                return commonResponse;
            }

            if (!order.ReceptImageFile.ContentType.Contains("image"))
            {
                commonResponse.StatusCode = 400;
                commonResponse.Message = "Upload image file";
                return commonResponse;
            }
            if (order.Number is null)
            {
                commonResponse.StatusCode = 400;
                commonResponse.Message = "Number field is required";
                return commonResponse;
            }
            if (order.Address is null)
            {
                commonResponse.StatusCode = 400;
                commonResponse.Message = "Address field is required";
                return commonResponse;
            }
            string imageFolderPath = Path.Combine(_env.WebRootPath, "assets", "images");

            string image = await order.ReceptImageFile.CreateImage(imageFolderPath, "website-images");

            string? userName = _http.HttpContext.User.Identity.Name;

            User? appUser = await _userManager.FindByNameAsync(userName);

            Order orderModel = new()
            {
                User = appUser,
                Status = null,
                Address = order.Address,
                Number = order.Number,
                ReceptImage = image,
                OrderItems = new List<OrderItem>(),
            };
            var baskets = await _context.Baskets
                   .Include(x => x.BasketItems).ThenInclude(x => x.Product).FirstOrDefaultAsync();

            foreach (var item in baskets.BasketItems)
            {
                orderModel.OrderItems.Add(new OrderItem
                {
                    Count = item.Count,
                    Order = orderModel,
                    OrderId = orderModel.Id,
                    ProductId = item.ProductId,
                    Product = item.Product,
                });
                var product = await _context.Products.Include(p => p.Images).Include(p => p.SubSubCategory).Include(p => p.BasketItems).FirstOrDefaultAsync(p => p.Id == item.ProductId);
                product.Count -= item.Count;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                if (item.Product.DiscountPrice != 0)
                {
                    order.TotalPrice += item.Product.DiscountPrice * item.Count;
                    orderModel.TotalPrice = order.TotalPrice;
                }
                else
                {
                    order.TotalPrice += item.Product.Price * item.Count;
                    orderModel.TotalPrice = order.TotalPrice;
                }
            }
            await _context.Orders.AddAsync(orderModel);
            _context.Baskets.Remove(baskets);
            await _context.SaveChangesAsync();

            commonResponse.Data = orderModel.Id;
            return commonResponse;
        }

        public async Task<Order> Get(int id)
        {
            var query = await _context.Orders
                   .Include(x => x.User)
                   .Include(x => x.OrderItems)
                   .ThenInclude(x => x.Product)
                   .ThenInclude(x => x.Images)
                   .FirstOrDefaultAsync(p => p.Id == id);

            return query;
        }

        public List<Order> GetAll()
        {
            List<Order>? orders = _context.Orders.Include(x => x.User)
                                                   .Include(x => x.OrderItems)
                                                   .ThenInclude(x => x.Product)
                                                   .OrderByDescending(x => x.Id)
                                                   .ToList();
            return orders;
        }

        public async Task Reject(int id)
        {
            Order? order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            order.Status = false;
            foreach (var item in order.OrderItems)
            {
                Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                int totalProductCount = product.ProductSizeColors.Sum(p => p.Count);

                totalProductCount -= item.Count;

                int totalCountAfterUpdate = totalProductCount / product.ProductSizeColors.Count();
                foreach (var sizeColor in product.ProductSizeColors)
                {
                    sizeColor.Count = totalCountAfterUpdate;
                }
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new List<Order>();
            }

            List<Order>? orders = await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Images)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductSizeColors).ThenInclude(x => x.Color)
                 .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductSizeColors).ThenInclude(x => x.Size)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            return orders;
        }
    }
}

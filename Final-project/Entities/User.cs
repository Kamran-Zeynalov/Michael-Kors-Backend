using Microsoft.AspNetCore.Identity;

namespace Final_project.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<Basket> BasketItems { get; set; } = null!;
        public List<Order>? Orders { get; set; }
    }
}

namespace Final_project.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public List<BasketItem> BasketItems { get; set; } = null!;
    }
}

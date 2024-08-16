namespace Final_project.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public decimal TotalPrice { get; set; }
        public bool? Status { get; set; }
        public string Number { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string ReceptImage { get; set; }
        public List<OrderItem>? OrderItems { get; set; } = new();
        public Order()
        {
            OrderItems = new();
        }
    }
}

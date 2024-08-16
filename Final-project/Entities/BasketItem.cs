namespace Final_project.Entities
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int BasketId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public Basket Basket { get; set; } = null!;
        public int Count { get; set; }
    }
}

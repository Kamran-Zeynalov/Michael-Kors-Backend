namespace Final_project.Entities
{
    public class ProductSizeColor : BaseEntity
    {
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public int Count { get; set; }
        public Product Product{ get; set; } = null!;
        public Color Color { get; set; } = null!;
        public Size Size { get; set; } = null!;
    }
}

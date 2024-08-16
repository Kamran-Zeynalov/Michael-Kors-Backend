namespace Final_project.Entities
{
    public class Product : BaseEntity
    {
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Count { get; set; }
        public List<Image> Images { get; set; }
        public SubSubCategory SubSubCategory { get; set; }
        public Brand Brand { get; set; }
        public List<ProductSizeColor> ProductSizeColors { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<WishlistItem> WishlistItems { get; set; }
        public List<Follower> Followers { get; set; }
        public Product()
        {
            ProductSizeColors = new();
        }

    }
}

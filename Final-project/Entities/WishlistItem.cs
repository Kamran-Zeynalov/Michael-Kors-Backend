﻿namespace Final_project.Entities
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; } = null!;
    }
}

namespace Final_project.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public List<WishlistItem> WishlistItems { get; set; } = null!;
    }
}

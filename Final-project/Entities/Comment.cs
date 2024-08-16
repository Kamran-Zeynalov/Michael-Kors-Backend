namespace Final_project.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime Time { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public bool AgreeToTerms { get; set; }
        public int Count { get; set; }
    }
}

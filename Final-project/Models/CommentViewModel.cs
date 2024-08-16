using Final_project.Entities;
using System.ComponentModel.DataAnnotations;

namespace Final_project.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string ReviewText { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        public bool AgreeToTerms { get; set; }
        public int Count { get; set; }
    }
}

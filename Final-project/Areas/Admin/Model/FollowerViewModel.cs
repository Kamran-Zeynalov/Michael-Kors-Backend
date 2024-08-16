using Final_project.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.Areas.Admin.Model
{
    public class FollowerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public List<Product>? Products { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImagePath { get; set; }
    }
}

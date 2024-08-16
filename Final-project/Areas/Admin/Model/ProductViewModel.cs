using Final_project.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_project.Areas.Admin.Model
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Count { get; set; }
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }
        public List<Image>? Images { get; set; }
        public ICollection<int>? ImageIds { get; set; }
        public int SubSubCategoryId { get; set; }
        public int BrandId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public List<SubSubCategory>? SubSubCategories { get; set; }
        public List<Brand>? Brands { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<Color>? Colors { get; set; }
        public string ColorSizeCount { get; set; }
    }
}

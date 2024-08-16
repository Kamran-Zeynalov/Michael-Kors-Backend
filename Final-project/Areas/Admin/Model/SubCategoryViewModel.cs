using Final_project.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Final_project.Areas.Admin.Model
{
    public class SubCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
    }
}

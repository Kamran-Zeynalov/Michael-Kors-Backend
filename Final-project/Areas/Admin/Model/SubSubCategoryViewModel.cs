using Final_project.Entities;

namespace Final_project.Areas.Admin.Model
{
    public class SubSubCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubCategoryId { get; set; }
        public List<SubCategory>? SubCategories { get; set; }
    }
}

namespace Final_project.Entities
{
    public class SubCategory : BaseEntity
    {
        public Category Category{ get; set; }
        public List<SubSubCategory>? SubSubCategories{ get; set; }
    }
}

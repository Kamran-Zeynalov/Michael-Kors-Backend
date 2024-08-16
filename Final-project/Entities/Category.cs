namespace Final_project.Entities
{
    public class Category:BaseEntity
    {
        public List<SubCategory>? SubCategories{ get; set; }
    }
}

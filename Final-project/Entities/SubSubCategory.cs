namespace Final_project.Entities
{
    public class SubSubCategory : BaseEntity
    {
        public SubCategory SubCategory{ get; set; }
        public List<Product> Products { get; set; }
    }
}

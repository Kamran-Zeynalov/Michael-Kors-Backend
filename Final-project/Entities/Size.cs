namespace Final_project.Entities
{
    public class Size : BaseEntity
    {
        public List<ProductSizeColor> ProductSizeColors { get; set; }
        public Size()
        {
            ProductSizeColors = new();

        }
    }
}

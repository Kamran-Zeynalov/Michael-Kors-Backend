namespace Final_project.Entities
{
    public class Color : BaseEntity
    {
        public string? Value { get; set; }
        public List<ProductSizeColor> ProductSizeColors { get; set; }
        public Color()
        {
            ProductSizeColors = new();
        }
    }
}

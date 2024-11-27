namespace LaptopStore.Data.Entities
{
    public class ProductAttribute
    {
        public int ProductAttributeID { get; set; }
        public string AttributeName { get; set; }
        public string AttributeUnit { get; set; }

        public ICollection<ProductSpecification> ProductSpecifications { get; set; }
    }
}

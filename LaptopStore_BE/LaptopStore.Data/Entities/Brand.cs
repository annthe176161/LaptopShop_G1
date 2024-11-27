namespace LaptopStore.Data.Entities
{
    public class Brand
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Product> Products { get; set; }
    }
}

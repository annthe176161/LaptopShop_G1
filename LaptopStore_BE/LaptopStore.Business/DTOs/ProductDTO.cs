namespace LaptopStore.Business.DTOs
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedDate { get; set; }
       
        public bool IsDeleted { get; set; } = false;

        
    }
}

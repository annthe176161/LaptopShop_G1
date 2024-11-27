namespace LaptopStore.Data.Entities
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int BrandID { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsDiscounted { get; set; }
        public double? DiscountPercent { get; set; }
        public int? DiscountID { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Brand Brand { get; set; }
        public Discount Discount { get; set; }
        public ICollection<ProductSpecification> ProductSpecifications { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}

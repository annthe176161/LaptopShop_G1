namespace LaptopStore.Data.Entities
{
    public class Discount
    {
        public int DiscountID { get; set; }
        public string DiscountCode { get; set; }
        public double DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<Product> Products { get; set; }
    }
}

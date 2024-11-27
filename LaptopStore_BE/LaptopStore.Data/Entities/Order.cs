namespace LaptopStore.Data.Entities
{
    public class Order
    {
        public int OrderID { get; set; }
        public string UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingAddress { get; set; }
        public string OrderStatus { get; set; } = "Pending";
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public string TrackingNumber { get; set; }
        public string Notes { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ApplicationUser User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

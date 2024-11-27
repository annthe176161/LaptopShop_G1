using System.ComponentModel.DataAnnotations;

namespace LaptopStore.Data.Entities
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Thông tin sản phẩm tại thời điểm đặt hàng
        [Required]
        public string ProductName { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        public decimal Subtotal { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}

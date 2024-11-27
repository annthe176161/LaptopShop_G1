using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.DTOs
{
    public class OrderDTO
    {
        public int OrderID { get; set; }
        public string UserID { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingMethod { get; set; } 
        public string Notes { get; set; }
        public string PhoneNumber { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}

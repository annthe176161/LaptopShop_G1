using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Entities
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal productPrice { get; set; }
        public string ImageURL { get; set; }
    }
}

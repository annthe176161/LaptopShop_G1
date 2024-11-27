using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.DTOs
{
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; }
        public string? Reason { get; set; }
    }
}

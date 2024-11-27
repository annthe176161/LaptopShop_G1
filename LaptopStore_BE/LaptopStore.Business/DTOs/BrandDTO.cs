using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.DTOs
{
    public class BrandDTO
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public bool IsDeleted { get; set; }
    }
}

using LaptopStore.Business.DTOs;
using LaptopStore.Business.Services.Base;
using LaptopStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.Services.Contracts
{
    public interface IBrandService : IBaseService<BrandDTO>
    {
        Task<IEnumerable<BrandDTO>> GetBrandsByConditionAsync(bool isDeleted);
    }
}

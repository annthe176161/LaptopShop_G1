using LaptopStore.Data.Contracts.Base;
using LaptopStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Contracts
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        Task<Brand> GetByNameAsync(string brandName);
    }
}

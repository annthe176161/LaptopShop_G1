using LaptopStore.Data.Contracts.Base;
using LaptopStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Contracts
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Lấy sản phẩm nổi bật (có khuyến mãi)
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();

        // Lấy sản phẩm theo thương hiệu
        Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);

        // Tìm kiếm nâng cao với các tiêu chí: từ khóa, thương hiệu, giá, và khuyến mãi
        Task<IEnumerable<Product>> GetAdvancedSearchAsync(string keyword, int? brandId, decimal? minPrice, decimal? maxPrice, string sortBy, bool? isDiscounted);

        // Lấy sản phẩm mới được tạo trong vòng 1 tháng
        Task<IEnumerable<Product>> GetNewProductsAsync();
    }
}

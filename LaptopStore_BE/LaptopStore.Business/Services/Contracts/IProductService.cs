using LaptopStore.Business.DTOs;
using LaptopStore.Business.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.Services.Contracts
{
    public interface  IProductService : IBaseService<ProductDTO>
    {
        // Lấy sản phẩm theo thương hiệu
        Task<IEnumerable<ProductDTO>> GetProductsByBrandAsync(int brandId);
        // Lấy sản phẩm nổi bật (có khuyến mãi)
        Task<IEnumerable<ProductDTO>> GetFeaturedProductsAsync();
        // Lấy sản phẩm mới được tạo trong vòng 1 tháng
        Task<IEnumerable<ProductDTO>> GetNewProductsAsync();
        // Tìm kiếm nâng cao với các tiêu chí: từ khóa, thương hiệu, giá, và khuyến mãi
        Task<IEnumerable<ProductDTO>> GetAdvancedSearchAsync(string keyword, int? brandId, decimal? minPrice, decimal? maxPrice, string sortBy, bool? isDiscounted);


    }
}

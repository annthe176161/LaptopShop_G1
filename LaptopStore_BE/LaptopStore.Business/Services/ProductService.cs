using LaptopStore.Business.DTOs;
using LaptopStore.Business.Services.Base;
using LaptopStore.Business.Services.Contracts;
using LaptopStore.Data.Contracts.Base;
using LaptopStore.Data.Entities;

namespace LaptopStore.Business.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Thêm sản phẩm mới
        public int Add(ProductDTO entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                var product = new Product
                {
                    Name = entity.Name,
                    BrandID = entity.BrandID,

                    Price = entity.Price,
                    Description = entity.Description,
                    StockQuantity = entity.StockQuantity,
                    ImageURL = entity.ImageURL,
                    CreatedDate = entity.CreatedDate,
                    IsDeleted = false
                };

                _unitOfWork.GenericRepository<Product>().Add(product);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new Exception("An error occurred while adding the product.", ex);
            }
        }


        // Cập nhật sản phẩm
        public int Update(ProductDTO entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                // Gán giá trị mặc định cho BrandName nếu nó là null
                entity.BrandName ??= string.Empty;
                var product = _unitOfWork.GenericRepository<Product>().GetById(entity.ProductID);
                if (product == null)
                    throw new KeyNotFoundException("Product not found.");

                product.Name = entity.Name;
                product.BrandID = entity.BrandID;
                product.Price = entity.Price;
                product.Description = entity.Description;
                product.StockQuantity = entity.StockQuantity;
                product.ImageURL = entity.ImageURL;
                product.CreatedDate = entity.CreatedDate;

                product.IsDeleted = entity.IsDeleted;

                _unitOfWork.GenericRepository<Product>().Update(product);
                return _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the product.", ex);
            }
        }

        // Xóa sản phẩm (chỉ đánh dấu là đã xóa)
        public int Delete(int id)
        {
            var product = _unitOfWork.GenericRepository<Product>().GetById(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            product.IsDeleted = true;  // Đánh dấu sản phẩm là đã xóa
            _unitOfWork.GenericRepository<Product>().Update(product);
            return _unitOfWork.SaveChanges();
        }

        // Lấy tất cả sản phẩm
        public IEnumerable<ProductDTO> GetAll()
        {
            var products = _unitOfWork.GenericRepository<Product>().GetAll();
            return products.Where(p => !p.IsDeleted) // Lọc sản phẩm chưa bị xóa
                           .Select(p => new ProductDTO
                           {
                               ProductID = p.ProductID,
                               Name = p.Name,
                               BrandID = p.BrandID,
                               BrandName = _unitOfWork.GenericRepository<Brand>().GetById(p.BrandID).BrandName,
                               Price = p.Price,
                               Description = p.Description,
                               StockQuantity = p.StockQuantity,
                               ImageURL = p.ImageURL,
                               CreatedDate = p.CreatedDate,

                               IsDeleted = p.IsDeleted,

                           });
        }

        // Lấy tất cả sản phẩm bất đồng bộ
        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _unitOfWork.GenericRepository<Product>().GetAllAsync();
            return products.Where(p => !p.IsDeleted)  // Lọc sản phẩm chưa bị xóa
                           .Select(p => new ProductDTO
                           {
                               ProductID = p.ProductID,
                               Name = p.Name,
                               BrandID = p.BrandID,
                               BrandName = _unitOfWork.GenericRepository<Brand>().GetById(p.BrandID).BrandName,
                               Price = p.Price,
                               Description = p.Description,
                               StockQuantity = p.StockQuantity,
                               ImageURL = p.ImageURL,
                               CreatedDate = p.CreatedDate,

                               IsDeleted = p.IsDeleted,

                           });
        }

        // Lấy sản phẩm theo ID
        public ProductDTO GetById(int id)
        {
            try
            {
                var product = _unitOfWork.GenericRepository<Product>().GetById(id);
                if (product == null || product.IsDeleted)
                    throw new KeyNotFoundException("Product not found or deleted.");

                return new ProductDTO
                {
                    ProductID = product.ProductID,
                    Name = product.Name,
                    BrandID = product.BrandID,
                    BrandName = _unitOfWork.GenericRepository<Brand>().GetById(product.BrandID)?.BrandName ?? "Unknown",
                    Price = product.Price,
                    Description = product.Description,
                    StockQuantity = product.StockQuantity,
                    ImageURL = product.ImageURL,
                    CreatedDate = product.CreatedDate,
                    IsDeleted = product.IsDeleted,
                    // Không trả về Discount

                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving product ID {id}.", ex);
            }
        }

        // Lấy sản phẩm theo thương hiệu (brandId)
        public async Task<IEnumerable<ProductDTO>> GetProductsByBrandAsync(int brandId)
        {
            var products = await _unitOfWork.GenericRepository<Product>().GetAllAsync();
            return products.Where(p => p.BrandID == brandId && !p.IsDeleted)  // Lọc theo BrandID và trạng thái không bị xóa
                           .Select(p => new ProductDTO
                           {
                               ProductID = p.ProductID,
                               Name = p.Name,
                               BrandID = p.BrandID,
                               Price = p.Price,
                               Description = p.Description,
                               StockQuantity = p.StockQuantity,
                               ImageURL = p.ImageURL,
                               CreatedDate = p.CreatedDate,

                               IsDeleted = p.IsDeleted,

                           });
        }

        // Lấy sản phẩm nổi bật
        public async Task<IEnumerable<ProductDTO>> GetFeaturedProductsAsync()
        {
            var products = await _unitOfWork.GenericRepository<Product>().GetAllAsync();
            return products.Where(p => !p.IsDeleted)  // Lọc sản phẩm giảm giá và chưa bị xóa
                           .Select(p => new ProductDTO
                           {
                               ProductID = p.ProductID,
                               Name = p.Name,
                               BrandID = p.BrandID,
                               Price = p.Price,
                               Description = p.Description,
                               StockQuantity = p.StockQuantity,
                               ImageURL = p.ImageURL,
                               CreatedDate = p.CreatedDate,

                               IsDeleted = p.IsDeleted,

                           });
        }

        //search nang cao 
        public async Task<IEnumerable<ProductDTO>> GetAdvancedSearchAsync(string keyword, int? brandId, decimal? minPrice, decimal? maxPrice, string sortBy, bool? isDiscounted)
        {
            var products = await _unitOfWork.GenericRepository<Product>().GetAllAsync();

            // Lọc sản phẩm theo từ khóa (tìm trong Name và Description)
            var query = products.Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword));
            }

            // Lọc theo thương hiệu
            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandID == brandId);
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            // Lọc theo sản phẩm có khuyến mãi
            //  if (isDiscounted.HasValue && isDiscounted.Value)
            // {
            // query = query.Where(p => p.IsDiscounted);
            // }

            // Sắp xếp theo tiêu chí
            query = sortBy switch
            {
                "price" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "created" => query.OrderByDescending(p => p.CreatedDate),
                _ => query.OrderBy(p => p.CreatedDate)
            };

            // Chuyển đổi sang ProductDTO
            return query.Select(p => new ProductDTO
            {
                ProductID = p.ProductID,
                Name = p.Name,
                BrandID = p.BrandID,
                Price = p.Price,
                Description = p.Description,
                StockQuantity = p.StockQuantity,
                ImageURL = p.ImageURL,
                CreatedDate = p.CreatedDate,

                IsDeleted = p.IsDeleted
            });
        }

        //san pham moi trong vong 1 thang
        public async Task<IEnumerable<ProductDTO>> GetNewProductsAsync()
        {
            // Lấy tất cả sản phẩm từ repository
            var products = await _unitOfWork.GenericRepository<Product>().GetAllAsync();

            // Lọc sản phẩm mới được tạo trong vòng 1 tháng
            var newProducts = products.Where(p => p.CreatedDate >= DateTime.Now.AddMonths(-1) && !p.IsDeleted);

            // Chuyển đổi sang ProductDTO và trả về kết quả
            return newProducts.Select(p => new ProductDTO
            {
                ProductID = p.ProductID,
                Name = p.Name,
                BrandID = p.BrandID,
                Price = p.Price,
                Description = p.Description,
                StockQuantity = p.StockQuantity,
                ImageURL = p.ImageURL,
                CreatedDate = p.CreatedDate,

                IsDeleted = p.IsDeleted
            });
        }



    }
}

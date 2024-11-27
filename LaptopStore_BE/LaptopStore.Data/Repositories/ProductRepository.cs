using LaptopStore.Data.Contexts;
using LaptopStore.Data.Contracts;
using LaptopStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LaptopStore.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Product entity)
        {
            _context.Products.Add(entity);
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.IsDeleted = true;
                _context.Products.Update(product);

            }
        }

        public async Task<IEnumerable<Product>> GetAdvancedSearchAsync(string keyword, int? brandId, decimal? minPrice, decimal? maxPrice, string sortBy, bool? isDiscounted)
        {
            var query = _context.Products
                   .Where(p => !p.IsDeleted)
                   .Include(p => p.Brand)
                   .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword));
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandID == brandId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            //    if (isDiscounted.HasValue && isDiscounted.Value)
            //  {
            //    query = query.Where(p => p.IsDiscounted);
            //}

            query = sortBy switch
            {
                "price" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "description" => query.OrderBy(p => p.Description),
                _ => query.OrderBy(p => p.CreatedDate)
            };

            return await query.ToListAsync();
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.Where(p => !p.IsDeleted).ToList();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
        }

        public Product GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.ProductID == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products.
                Where(p => !p.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetNewProductsAsync()
        {
            return await _context.Products
                 .Where(p => p.CreatedDate >= DateTime.Now.AddMonths(-1) && !p.IsDeleted)
                 .Include(p => p.Brand)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
        {
            return await _context.Products
                .Where(p => p.BrandID == brandId && !p.IsDeleted)
                .Include(p => p.Brand)
                .ToListAsync();
        }



        public IQueryable<Product> GetQuery()
        {
            return _context.Products.Where(p => !p.IsDeleted);
        }

        public void Update(Product entity)
        {
            _context.Products.Update(entity);
        }
    }
}

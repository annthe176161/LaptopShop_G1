using LaptopStore.Business.DTOs;
using LaptopStore.Business.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaptopStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public ActionResult<ProductDTO> GetProductById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // GET: api/products/brand/{brandId}
        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByBrand(int brandId)
        {
            var products = await _productService.GetProductsByBrandAsync(brandId);
            return Ok(products);
        }

        // GET: api/products/featured
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetFeaturedProducts()
        {
            var products = await _productService.GetFeaturedProductsAsync();
            return Ok(products);
        }

        // GET: api/products/new
        [HttpGet("new")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetNewProducts()
        {
            var products = await _productService.GetNewProductsAsync();
            return Ok(products);
        }

        // GET: api/products/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAdvancedSearch(
            [FromQuery] string keyword,
            [FromQuery] int? brandId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string sortBy,
            [FromQuery] bool? isDiscounted)
        {
            var products = await _productService.GetAdvancedSearchAsync(keyword, brandId, minPrice, maxPrice, sortBy, isDiscounted);
            return Ok(products);
        }

        // POST: api/products
        [HttpPost]
        public ActionResult<ProductDTO> AddProduct([FromBody] ProductDTO productDto)
        {
            var result = _productService.Add(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = productDto.ProductID }, productDto);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            if (id != productDto.ProductID)
            {
                return BadRequest();
            }

            _productService.Update(productDto);
            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            _productService.Delete(id);
            return NoContent();
        }
    }
}

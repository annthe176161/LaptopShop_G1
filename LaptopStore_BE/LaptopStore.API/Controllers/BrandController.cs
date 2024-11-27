using LaptopStore.Business.DTOs;
using LaptopStore.Business.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaptopStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {

        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // GET: api/brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetAllBrands()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        // GET: api/brands/{id}
        [HttpGet("{id}")]
        public ActionResult<BrandDTO> GetBrandById(int id)
        {
            var brand = _brandService.GetById(id);
            if (brand == null)
            {
                return NotFound();
            }
            return Ok(brand);
        }

        // POST: api/brands
        [HttpPost]
        public ActionResult<BrandDTO> AddBrand([FromBody] BrandDTO brandDto)
        {
            var result = _brandService.Add(brandDto);
            return CreatedAtAction(nameof(GetBrandById), new { id = brandDto.BrandID }, brandDto);
        }

        // PUT: api/brands/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateBrand(int id, [FromBody] BrandDTO brandDto)
        {
            if (id != brandDto.BrandID)
            {
                return BadRequest();
            }

            _brandService.Update(brandDto);
            return NoContent();
        }

        // DELETE: api/brands/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteBrand(int id)
        {
            var brand = _brandService.GetById(id);
            if (brand == null)
            {
                return NotFound();
            }

            _brandService.Delete(id);
            return NoContent();
        }
    }

}


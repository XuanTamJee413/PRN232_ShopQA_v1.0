using Business.DTO;
using Business.Iservices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopQAPresentation.Controllers.Category
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null) return NotFound();
            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrandDTO brandDto)
        {
            try
            {
                var created = await _brandService.AddAsync(brandDto);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandDTO brandDto)
        {
            if (id != brandDto.Id) return BadRequest();

            try
            {
                var success = await _brandService.UpdateAsync(brandDto);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _brandService.DeleteAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var result = await _brandService.SearchByNameAsync(name);
            return Ok(result);
        }

        [HttpGet("sort")]
        public async Task<IActionResult> Sort([FromQuery] bool desc = false)
        {
            var result = await _brandService.SortByNameAsync(desc);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] string? search,[FromQuery] string? sort,[FromQuery] int page = 1)
        {
            var brands = await _brandService.GetPagedAsync(search, sort, page);
            return Ok(brands);
        }

    }
}

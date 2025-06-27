using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopQAPresentation.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult getAllCategory()
        {
            try
            {
                return Ok(_categoryService.getAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDTO category)
        {
            try
            {
                await _categoryService.AddCategoryAsync(category);
                return Ok(new { message = "Thêm danh mục thành công." });
            }
            catch (InvalidOperationException ex) 
            {
                return Conflict(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO category)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(id, category);
                return Ok(new { message = "Cập nhật danh mục thành công." });
            }
            catch (InvalidOperationException ex) 
            {
                return Conflict(new { message = ex.Message }); 
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _categoryService.DeleteCategoryAsync(id);

                if (!deleted)
                {
                    
                    return BadRequest("Không thể xóa danh mục vì còn sản phẩm liên quan.");
                }

                return Ok("Xóa danh mục thành công.");
            }
            catch (Exception ex)
            {
                
                return NotFound(ex.Message);
            }
        }

        // Search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            try
            {
                var result = await _categoryService.SearchCategoriesByNameAsync(keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Sort
        [HttpGet("sort")]
        public async Task<IActionResult> Sort([FromQuery] bool sortAsc = true)
        {
            try
            {
                var result = await _categoryService.SortCategoriesByNameAsync(sortAsc);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

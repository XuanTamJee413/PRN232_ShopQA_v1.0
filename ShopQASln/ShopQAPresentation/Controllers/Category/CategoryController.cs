﻿using Business.DTO;
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

        public CategoryController(ICategoryService categoryService) { 
        this._categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult getAllCategory() {
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
        public async Task<IActionResult> Create( CategoryDTO category)
        {
            await _categoryService.AddCategoryAsync(category);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryDTO category)
        {
           
            await _categoryService.UpdateCategoryAsync(id, category);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _categoryService.DeleteCategoryAsync(id);

                if (!deleted)
                {
                    // Category còn sản phẩm nên không xóa được
                    return BadRequest("Không thể xóa danh mục vì còn sản phẩm liên quan.");
                }

                return Ok("Xóa danh mục thành công.");
            }
            catch (Exception ex)
            {
                // Không tìm thấy category hoặc lỗi khác
                return NotFound(ex.Message);
            }
        }

    }
}

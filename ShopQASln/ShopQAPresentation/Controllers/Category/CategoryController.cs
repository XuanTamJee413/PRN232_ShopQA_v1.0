using Business.DTO;
using Business.Iservices;
using DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using CategoryDomain = Domain.Models.Category;

namespace ShopQAPresentation.Controllers.Category
{
    [Route("odata/Category")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ODataController 
    {
        private readonly ShopQADbContext _context;

        public CategoryController(ShopQADbContext context)
        {
            _context = context;
        }

        // Phương thức Get() để lấy danh sách
        [EnableQuery]
        public IQueryable<CategoryDomain> Get()
        {
            return _context.Categories;
        }

        // Phương thức Get(key) để lấy một đối tượng
        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            var category = await _context.Categories.FindAsync(key);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // Phương thức Post() để tạo mới
        public async Task<IActionResult> Post([FromBody] CategoryDomain category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Created(category);
        }

        // Phương thức Put() để cập nhật toàn bộ
        public async Task<IActionResult> Put(int key, [FromBody] CategoryDomain updatedCategory)
        {
            if (key != updatedCategory.Id)
            {
                return BadRequest("ID không khớp.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(updatedCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.Id == key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        
        public async Task<IActionResult> Delete(int key)
        {
            var categoryToDelete = await _context.Categories.FindAsync(key);
            if (categoryToDelete == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
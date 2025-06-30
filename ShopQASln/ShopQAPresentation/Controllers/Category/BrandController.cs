using DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

using BrandDomain = Domain.Models.Brand;

namespace ShopQAPresentation.Controllers.Brand
{
    

    // [Authorize(Roles = "Admin")] 
    public class BrandController : ODataController
    {
        private readonly ShopQADbContext _context;

        public BrandController(ShopQADbContext context)
        {
            _context = context;
        }

     
        [EnableQuery]
        public IQueryable<BrandDomain> Get()
        {
            return _context.Brands;
        }

       
        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
            var brand = await _context.Brands.FindAsync(key);
            if (brand == null)
            {
                return NotFound();
            }
            return Ok(brand);
        }

        
        public async Task<IActionResult> Post([FromBody] BrandDomain brand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return Created(brand);
        }

       
        public async Task<IActionResult> Put(int key, [FromBody] BrandDomain updatedBrand)
        {
            if (key != updatedBrand.Id)
            {
                return BadRequest("ID không khớp.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(updatedBrand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Brands.Any(e => e.Id == key))
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
            var brandToDelete = await _context.Brands.FindAsync(key);
            if (brandToDelete == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brandToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
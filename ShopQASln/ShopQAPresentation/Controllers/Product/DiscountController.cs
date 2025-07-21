using DataAccess.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using DiscountDomain = Domain.Models.Discount;
using Business.DTO;

namespace ShopQAPresentation.Controllers.Product
{
    // [Authorize(Roles = "Admin")] 
    public class DiscountController : ODataController
    {
        private readonly ShopQADbContext _context;

        public DiscountController(ShopQADbContext context)
        {
            _context = context;
        }

       
        [EnableQuery]
        public IQueryable<DiscountDomain> Get()
        {
            
            return _context.Discounts;
        }

     
        [EnableQuery]
        public async Task<IActionResult> Get(int key)
        {
           
            var discount = await _context.Discounts.SingleOrDefaultAsync(d => d.Id == key);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }


        public async Task<IActionResult> Post([FromBody] DiscountCreateDTO discountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (discountDto.Amount > 40)
            {
                return BadRequest(new { message = "Phần trăm giảm giá không được vượt quá 40%." });
            }

            if (discountDto.StartDate >= discountDto.EndDate)
            {
                return BadRequest(new { message = "Ngày bắt đầu phải trước ngày kết thúc." });
            }
            var productVariantExists = await _context.ProductVariants.AnyAsync(pv => pv.Id == discountDto.ProductVariantId);
            if (!productVariantExists)
            {
                return BadRequest(new { message = $"Không tìm thấy biến thể sản phẩm với ID = {discountDto.ProductVariantId}." });
            }
           

           
            var newDiscount = new DiscountDomain
            {
                Amount = discountDto.Amount,
                StartDate = discountDto.StartDate,
                EndDate = discountDto.EndDate,
                Status = discountDto.Status,
                ProductVariantId = discountDto.ProductVariantId
            };

            _context.Discounts.Add(newDiscount);
            await _context.SaveChangesAsync();

            return Created(newDiscount);
        }

        
        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] DiscountUpdateDTO discountDto)
        {
            if (key != discountDto.Id)
            {
                return BadRequest("ID trong URL và trong body không khớp.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          
            var existingDiscount = await _context.Discounts.FindAsync(key);
            if (existingDiscount == null)
            {
                return NotFound();
            }

           
            existingDiscount.Amount = discountDto.Amount;
            existingDiscount.StartDate = discountDto.StartDate;
            existingDiscount.EndDate = discountDto.EndDate;
            existingDiscount.Status = discountDto.Status;
            existingDiscount.ProductVariantId = discountDto.ProductVariantId;

            if (discountDto.Amount > 40)
            {
                return BadRequest(new { message = "Phần trăm giảm giá không được vượt quá 40%." });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Discounts.Any(e => e.Id == key))
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



        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var discountToDelete = await _context.Discounts.FindAsync(key);
            if (discountToDelete == null)
            {
                return NotFound();
            }

            _context.Discounts.Remove(discountToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
using Business.DTO;
using Business.Iservices;
using Business.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Linq;

namespace ShopQAPresentation.Controllers
{
    public class ProductController : ODataController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [EnableQuery]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var products = _productService.GetQueryableVisibleProducts();
            return Ok(products);
        }
        [EnableQuery]
        [AllowAnonymous]
        public IActionResult Get([FromODataUri] int key)
        {
            var product = _productService.GetQueryableVisibleProducts()
                                         .Where(p => p.Id == key);
            return Ok(SingleResult.Create(product));
        }

        [HttpGet("api/Product")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult GetAllProduct(string? name, int? categoryId, decimal? startPrice, decimal? toPrice)
        {
            var products = _productService.GetAllProduct(name, categoryId, startPrice, toPrice);
            return Ok(products);
        }

        [HttpGet("api/Product/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult GetProductById(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("api/Product")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateReqDTO productDTO)
        {
            try
            {
                var result = await _productService.AddProduct(productDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("api/Product/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductCreateReqDTO productDTO)
        {
            try
            {
                var result = await _productService.UpdateProduct(id, productDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("api/Product/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                return Ok(_productService.DeleteProduct(id));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("api/Product/variant/{variantId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateVariantWithInventory(int variantId, [FromForm] ProductVariantWithInventoryUpdateDTO dto)
        {
            try
            {
                var result = await _productService.UpdateVariantWithInventory(variantId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("api/Product/variant")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVariant([FromForm] ProductVariantCreateDTO dto) // Changed to [FromForm] and async Task<IActionResult>
        {
            try
            {
                var result = await _productService.CreateVariant(dto);
                return Ok(result);
            }
            catch (ArgumentException ex) // Good practice to catch specific exceptions
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
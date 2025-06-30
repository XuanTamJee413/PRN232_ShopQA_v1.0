using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ShopQAPresentation.Controllers.Product
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class ProductController : ODataController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService ;
        }
        // tamnx test get bang odata https://localhost:7101/odata/Product?$expand=Variants,Category,Brand
        [EnableQuery]
        public IActionResult Get()
        {
            var products = _productService.GetQueryableVisibleProducts();
            return Ok(products);
        }
        // tamnx test get by id bang odata https://localhost:7101/odata/Product(id)?$expand=Variants,Category,Brand
        [EnableQuery]
        public IActionResult Get([FromODataUri] int key)
        {
            var product = _productService.GetQueryableVisibleProducts()
                                         .Where(p => p.Id == key);
            return Ok(SingleResult.Create(product));
        }


        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult GetAllProduct(string? name, int? categoryId, decimal? startPrice, decimal? toPrice)
        {
                var products = _productService.GetAllProduct(name, categoryId, startPrice, toPrice);
                return Ok(products);
          
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
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

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public IActionResult AddProduct(ProductCreateReqDTO productDTO)
        {
            try
            {
                var result = _productService.AddProduct(productDTO);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public IActionResult UpdateProduct(int id, ProductCreateReqDTO productDTO)
        {
            try
            {
                var updatedProduct = _productService.UpdateProduct(id, productDTO);
                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
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

        [HttpPut("variant/{variantId}")]
        public IActionResult UpdateVariantWithInventory(int variantId, [FromBody] ProductVariantWithInventoryUpdateDTO dto)
        {
            try
            {
                var result = _productService.UpdateVariantWithInventory(variantId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPost("variant")]
        public IActionResult CreateVariant([FromBody] ProductVariantCreateDTO dto)
        {
            try
            {
                var result = _productService.CreateVariant(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

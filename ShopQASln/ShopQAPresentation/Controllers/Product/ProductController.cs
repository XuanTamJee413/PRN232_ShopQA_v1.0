using Business.DTO;
using Business.Iservices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopQAPresentation.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService ;
        }
        [HttpGet]
        public IActionResult GetAllProduct(string? name, int? categoryId, decimal? startPrice, decimal? toPrice)
        {
            try
            {
                var products = _productService.GetAllProduct(name, categoryId, startPrice, toPrice);
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.getProductById(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");  
            }

            return Ok(product);
        }
        [HttpPost]
        public IActionResult AddProduct(ProductDTO productDTO) {
            try
            {
                return Ok(_productService.AddProduct(productDTO));

            }
            catch (ArgumentException ex) {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductDTO productDTO)
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
    }
}

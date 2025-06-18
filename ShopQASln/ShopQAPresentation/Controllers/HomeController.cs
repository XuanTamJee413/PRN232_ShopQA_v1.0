using Business.Iservices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ShopQAPresentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;

        public HomeController(IProductService productService, IProductVariantService productVariantService)
        {
            _productService = productService;
            _productVariantService = productVariantService;
        }

        [HttpGet("productlist")]
        [EnableQuery]
        public IActionResult GetVisibleProducts()
        {
            var products = _productService.GetVisibleProducts(null, null, null).AsQueryable();
            return Ok(products);
        }

        [HttpGet("variants")]
        public async Task<IActionResult> GetVariantsByProductId([FromQuery] int productId)
        {
            var variants = await _productVariantService.GetVariantsByProductIdAsync(productId);
            return Ok(variants);
        }


    }
}

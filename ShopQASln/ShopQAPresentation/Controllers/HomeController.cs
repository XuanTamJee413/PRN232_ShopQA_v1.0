using Business.Iservices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ShopQAPresentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ODataController
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
        [HttpGet("productvariants")]
        [EnableQuery]
        public IQueryable<ProductVariant> GetVariantsByProductIdAndOptions(ODataQueryOptions<ProductVariant> options, [FromQuery] int productId)
        {
            var variants = _productVariantService.GetVariantsByProductIdAsync(productId).Result.AsQueryable();

            IQueryable results = options.ApplyTo(variants);

            return results as IQueryable<ProductVariant>;
        }

    }
}

using KAShop.Bll.Service;
using KAShop.Dal.DTOs.Request;
using KAShop.Pl.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client.Extensions.Msal;

namespace KAShop.Pl.Areas.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ProductsController(IProductService productService, IStringLocalizer<SharedResource> Localizer)
        {
            _productService = productService;
            _localizer = Localizer;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetAllProductsForAdmin();
            return Ok(new { message = _localizer["success"].Value, response });
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm]ProductRequest request)
        {
            var response = await _productService.CreateProduct(request);
            return Ok(new { message = _localizer["success"].Value,response });

        }
    }
}

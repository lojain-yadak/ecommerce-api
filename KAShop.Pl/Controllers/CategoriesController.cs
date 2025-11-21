using KAShop.Dal.Data;
using KAShop.Dal.DTOs.Request;
using KAShop.Pl.Resources;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KAShop.Pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        ApplicationDbContext context;
        public CategoriesController(ApplicationDbContext context,IStringLocalizer<SharedResource> localizer)
        {
            this.context = context;
            _localizer = localizer;
        }
        [HttpGet("")]
        public IActionResult index() {
            var categories = context.Categories.ToList();
            var response = categories.Adapt<List<CategoryRequest>>();
            return Ok(new { message = _localizer["success"].Value, response });
        }

        [HttpPost("")]
        public IActionResult Create() {
            return Ok();
        }
       
    }
}

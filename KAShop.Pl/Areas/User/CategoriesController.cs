using KAShop.Bll.Service;
using KAShop.Dal.DTOs.Request;
using KAShop.Pl.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KAShop.Pl.Areas.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _category;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CategoriesController(ICategoryService category,
            IStringLocalizer<SharedResource> localizer)
        {
            _category = category;
            _localizer = localizer;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery]string lang="en")
        {
            var response =await _category.GetAllCategoriesForUser(lang);
            return Ok(new { message = _localizer["success"].Value, response });
        }
       
    }
}

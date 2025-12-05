using KAShop.Bll.Service;
using KAShop.Dal.Data;
using KAShop.Dal.DTOs.Request;
using KAShop.Dal.DTOs.Response;
using KAShop.Dal.Models;
using KAShop.Dal.Repository;
using KAShop.Pl.Resources;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace KAShop.Pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ICategoryService _categoryService;


        public CategoriesController(IStringLocalizer<SharedResource> localizer,ICategoryService categoryService)
        {
            _localizer = localizer;
            _categoryService = categoryService;
        }
        [HttpGet("")]
        public IActionResult index() {

            var response = _categoryService.GetAllCategories();
            return Ok(new { message = _localizer["success"].Value, response });
        }

        [HttpPost("")]
        public IActionResult Create(CategoryRequest request) {

            var response = _categoryService.CreateCategory(request);
                     
            return Ok(new {message = _localizer["success"].Value });
        }
       
    }
}

using ecommerceApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        IOS _Service;
        public PlatformsController(IOS _Services)
        {
            _Service = _Services;
        }
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(_Service.Run());
        }
    }
}

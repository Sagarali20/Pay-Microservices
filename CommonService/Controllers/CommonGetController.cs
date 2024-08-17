using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommonService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonGetController : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllAccountType()
        {

            return Ok("ok");
        }
    }
}

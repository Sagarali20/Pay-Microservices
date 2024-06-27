using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SendMoneyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        // GET: api/<CustomerController>
        [HttpGet]
        //[Authorize(Roles = "Administrator")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpGet]
        [Route("GetAll")]
       // [Authorize(Roles = "User")]
        public IEnumerable<string> GetAll()
        {
            return new string[] { "valuerrr", "value2rrr" };
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentService.Controllers
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
            int userId = CurrentUserInfo.UserId();
            string  UserName = CurrentUserInfo.UserName();

            // Attempt to get the 'userid' header value
            // Handle the case where the 'userid' header is missing

            return new string[] { userId.ToString(), UserName };
            
        }
        [HttpGet]
        [Route("GetAll")]
       // [Authorize(Roles = "User")]
        public IEnumerable<string> GetAll()
        {
            return new string[] { "valuerrr", "value2rrr" };
        }

        [HttpGet("[action]")]
        public IEnumerable<string> GetAllNew()
        {
            return new string[] { "valuerrr", "value2rrr" };
        }



    }
}

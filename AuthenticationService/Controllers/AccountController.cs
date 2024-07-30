using AuthenticationService.Utils;
using JwtAuthenticationManager;
using JwtAuthenticationManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;

        public AccountController(JwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;
        }
        [HttpPost]
        public ActionResult<AuthenticationResponse?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            //string password = MD5Encryption.GetMD5HashData(authenticationRequest.Password);
            //var authenticationResponse = _jwtTokenHandler.GenerateJwtToken(authenticationRequest);
            //if (authenticationResponse == null) return Unauthorized();
            return null;
        }
    }
}

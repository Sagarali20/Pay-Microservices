using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Utils;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService.Models;
using Consul;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Helpers.Interface;
using AuthenticationService.Helpers;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginController(IMediator mediator, JwtTokenHandler jwtTokenHandler)
        {
            _mediator = mediator;
            _jwtTokenHandler = jwtTokenHandler;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveUser(AddOrEditUser command)
        {
           
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginUser command)
        {
            try
            {
                command.Password = MD5Encryption.GetMD5HashData(command.Password);
                AuthenticationResponse authenticationResponse = new AuthenticationResponse();
                User user = await _mediator.Send(command);
                if (user is not null)
                {
                    if (user.tx_password == command.Password)
                    {
                        authenticationResponse = _jwtTokenHandler.GenerateJwtToken(user.id_user_key,user.tx_mobile_no,0,"");
                        var res = new
                        {
                            result = true,
                            token = authenticationResponse,
                            userFullName = user.tx_first_name + " " + user.tx_last_name,
                            permission=user.Permission
                        };
                        return Ok(res);

                    }
                    return Ok(new { result = false, message = "Invalid password." });
                }
                return Ok(new { result = false, message = "User not found!" });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LogOut()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                foreach (var claim in identity.Claims.ToList())
                {
                    identity.RemoveClaim(claim);
                }
            }
            return Ok("ok");
        }  
    }
}

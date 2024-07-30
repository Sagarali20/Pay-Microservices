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

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;

        public LoginController(IMediator mediator, JwtTokenHandler jwtTokenHandler, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _jwtTokenHandler = jwtTokenHandler;
            _currentUserService = currentUserService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveUser(AddOrEditUser command)
        {
            var userid = _currentUserService;
           
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginUser command)
        {
      
            try
            {
                command.TxPassword = MD5Encryption.GetMD5HashData(command.TxPassword);
                AuthenticationResponse authenticationResponse = new AuthenticationResponse();
                User user = await _mediator.Send(command);
                if (user is not null)
                {
                    if (user.tx_password == command.TxPassword)
                    {
                        authenticationResponse = _jwtTokenHandler.GenerateJwtToken(user.id_user_key,user.tx_mobile_no,0,"");
                        var res = new
                        {
                            result = true,
                            token = authenticationResponse,
                            userFullName = user.tx_first_name + " " + user.tx_last_name
                        };
                        var userId = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
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
        public async Task<IActionResult> TestU(Test command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }  
    }
}

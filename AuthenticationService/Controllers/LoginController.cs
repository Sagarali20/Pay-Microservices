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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using AuthenticationService.Application.Request.Login.Query;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuthenticationService.Controllers
{
    [Route("api/Authentication")]
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
                            userFullName = user.tx_first_name + " " + user.tx_last_name,
                            token = authenticationResponse.JwtToken,
                            permission=user.Permission
                        };
                        return Ok(res);

                    }
                    return BadRequest(new { result = false, message = "Invalid password." });
                }
                return NotFound(new { result = false, message = "User not found!" });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Refresh()
        {
            int userId = CurrentUserInfo.UserId();

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (token is null)
            {
                return BadRequest("invalid client request");
            }
            try
            {
                var pricipal = GetPrincipleFromExpiredToken(token);

                var result = await _mediator.Send(new LoginUser(pricipal.Identity.Name.ToString(), "" ));
                if(result is not null)
                {
                    return Ok(new { token = _jwtTokenHandler.GenerateJwtToken(result.id_user_key, result.tx_mobile_no, 0, "").JwtToken.ToString() });

                }
                return BadRequest(new { token = "Invalid user!" });
                //var newAccessToken = createJwt(user);
            }
            catch (Exception ex)
            {
                return  Ok(ex.Message);
            }
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(JwtTokenHandler.JWT_SECURITY_KEY);

            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenhandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenhandler.ValidateToken(token, tokenValidationParameter, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("this is invalid token");
            return principal;

        }


        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {

            return Ok("ok");
        }  
    }
}
